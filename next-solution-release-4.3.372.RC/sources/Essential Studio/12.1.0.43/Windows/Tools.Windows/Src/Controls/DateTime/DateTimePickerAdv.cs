#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.CodeDom;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Syncfusion.Drawing;
using Syncfusion.Runtime.InteropServices;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Design;
using Syncfusion.Windows.Forms.Tools;
using Microsoft.Win32;

namespace Syncfusion.Windows.Forms.Tools
{
    internal class DateTimePickerUpDownButton :
        ScrollButtons
    {
        private bool m_bMouseHovered;
        private Point m_pMouseLocation;
        private bool m_bMousePressed;

        protected override void OnMouseHover(EventArgs e)
        {
            m_bMouseHovered = true;
            this.Invalidate();
            base.OnMouseHover(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            m_bMouseHovered = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_pMouseLocation = new Point(e.X, e.Y);
            m_bMousePressed = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_bMousePressed = false;
            base.OnMouseUp(e);
        }

        protected override void DrawOffice2007ScrollButtonBackground(Graphics g, Rectangle rect, ButtonState buttonState)
        {
            Brush brush;

            if (this.Enabled)
            {
                Blend blend = new Blend();
                Color color1 = Color.Empty, color2 = Color.Empty;
                m_arrowColor = m_office2007ColorTable.DataTimePickerDropDownArrowColor;
                bool pushed = m_bMousePressed && rect.Contains(m_pMouseLocation);

                if (pushed)
                {
                    color1 = m_office2007ColorTable.DataTimePickerDropDownSelectedLightColor;
                    color2 = m_office2007ColorTable.DataTimePickerDropDownSelectedDarkColor;
                }
                else if (m_bMouseHovered)
                {
                    color1 = m_office2007ColorTable.DataTimePickerDropDownHighLightLightColor;
                    color2 = m_office2007ColorTable.DataTimePickerDropDownHighLightDarkColor;
                }
                else if (this.Selected)
                {
                    color1 = m_office2007ColorTable.DataTimePickerDropDownLightColor;
                    color2 = m_office2007ColorTable.DataTimePickerDropDownDarkColor;
                    if (this.Office2007ColorScheme == Office2007Theme.Black)
                    {
                        m_arrowColor = Color.White;
                    }
                }
                else if (this.ButtonState == ButtonState.Normal)
                {
                    brush = new SolidBrush(Color.White);
                }

                blend.Positions = new float[] { 0.0F, 0.45F, 0.45F + 0.001F, 1.0F };

                if (pushed)
                {
                    blend.Factors = new float[] { 0.0F, 0.8F, 1.0F, 0.4F };
                }
                else
                {
                    blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.0F };
                }

                brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                (brush as LinearGradientBrush).Blend = blend;
            }
            else
            {
                brush = new SolidBrush(this.BackColor);
            }

            g.FillRectangle(brush, rect);
        }
        protected override void DrawOffice2007ScrollButtonArrow(Graphics g, Rectangle rect, System.Windows.Forms.ScrollButton scroll, ButtonState buttonState)
        {
            if (this.VSLikeButton)
            {
                this.DrawVSLikeButtonArrow(g, rect, scroll, m_arrowColor, buttonState);
            }
            else
            {
                base.DrawOffice2007ScrollButtonArrow(g, rect, scroll, buttonState);
            }
        }
    }

    /// <summary>
    /// Drop button for drop down date calendar.
    /// </summary>
    internal class DateTimePickerDropButton :
        ThemedScrollButton
    {
        #region Members

        /// <summary>
        /// Indicates whether the calendar is dropped.
        /// </summary>
        private bool m_bDropped = false;

        /// <summary>
        /// Indicates whether the calendar is selected (highlighted).
        /// </summary>
        private bool m_bSelected;

        /// <summary>
        /// Normal mode backcolor.
        /// </summary>
        private Color m_normalColor = Color.Empty;

        /// <summary>
        /// Pressed mode backcolor.
        /// </summary>
        private Color m_pressedColor = Color.Empty;

        /// <summary>
        /// Selected mode backcolor.
        /// </summary>
        private Color m_selectedColor = Color.Empty;

        /// <summary>
        /// Arrow color.
        /// </summary>
        private Color m_arrowColor = Office2007Colors.Default.DataTimePickerDropDownArrowColor;
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the button dropped is state.
        /// </summary>
        public bool Dropped
        {
            get
            {
                return m_bDropped;
            }
            set
            {
                m_bDropped = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button selected (highlighted) state.
        /// </summary>
        public bool Selected
        {
            get
            {
                return m_bSelected;
            }
            set
            {
                m_bSelected = value;
            }
        }

        /// <summary>
        ///  Gets or sets normal mode back color.
        /// If color empty then use default color.
        /// </summary>
        public Color NormalColor
        {
            get
            {
                return m_normalColor;
            }
            set
            {
                if (m_normalColor != value)
                {
                    m_normalColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets pressed mode backcolor.
        /// If color empty then use default color.
        /// </summary>
        public Color PressedColor
        {
            get
            {
                return m_pressedColor;
            }
            set
            {
                if (m_pressedColor != value)
                {
                    m_pressedColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        ///  Gets or sets selected mode backcolor.
        /// If color empty then use default color.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return m_selectedColor;
            }
            set
            {
                if (m_selectedColor != value)
                {
                    m_selectedColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets the color of the background.
        /// </summary>
        /// <param name="buttonState">State of the button.</param>
        /// <returns>Return back color</returns>
        /// <override/>
        protected override Color GetBackColor(ButtonState buttonState)
        {
            Color backColor = Color.Empty;
            if (buttonState == ButtonState.Pushed)
            {
                backColor = this.PressedColor;
            }
            else if (mouseOver)
            {
                backColor = this.SelectedColor;
                mouseOver = false;
            }
            else
            {
                backColor = this.NormalColor;
            }

            Color retColor;
            if (backColor != Color.Empty)
            {
                retColor = backColor;
            }
            else
            {
                retColor = base.GetBackColor(buttonState);
            }

            return retColor;
        }

        public override Color ArrowColor
        {
            get
            {
                return m_arrowColor;
            }
        }

        protected override Brush GetBackGroundBrush(ButtonState buttonState)
        {
            if (this.Style == VisualStyle.Office2007)
            {
                LinearGradientBrush brush = null;
                Blend blend = new Blend();

                Color color1 = Color.Empty, color2 = Color.Empty;

                m_arrowColor = office2007ColorTable.DataTimePickerDropDownArrowColor;

                if (buttonState == ButtonState.Pushed)
                {
                    color1 = office2007ColorTable.DataTimePickerDropDownSelectedLightColor;
                    color2 = office2007ColorTable.DataTimePickerDropDownSelectedDarkColor;
                }
                else if (this.mouseOver)
                {
                    color1 = office2007ColorTable.DataTimePickerDropDownHighLightLightColor;
                    color2 = office2007ColorTable.DataTimePickerDropDownHighLightDarkColor;
                }
                else if (this.Selected && !this.mouseOver)
                {
                    color1 = office2007ColorTable.DataTimePickerDropDownLightColor;
                    color2 = office2007ColorTable.DataTimePickerDropDownDarkColor;
                    if (this.Office2007Theme == Office2007Theme.Black)
                    {
                        m_arrowColor = Color.White;
                    }
                }
                else if (buttonState == ButtonState.Normal)
                {
                    return new SolidBrush(Color.White);
                }

                blend.Positions = new float[] { 0.0F, 0.45F, 0.45F + 0.001F, 1.0F };

                if (buttonState == ButtonState.Pushed)
                    blend.Factors = new float[] { 0.0F, 0.8F, 1.0F, 0.4F };
                else
                    blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.0F };

                Rectangle rect = this.ClientRectangle;
                rect.Width -= 3;
                rect.Height--;

                brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                brush.Blend = blend;

                return brush;
            }
            else if (this.Style == VisualStyle.Office2010)
            {
                LinearGradientBrush brush = null;
                Blend blend = new Blend();

                Color color1 = Color.Empty, color2 = Color.Empty;

                m_arrowColor = office2010ColorTable.DataTimePickerDropDownArrowColor;

                if (buttonState == ButtonState.Pushed)
                {
                    color1 = office2010ColorTable.DataTimePickerDropDownSelectedLightColor;
                    color2 = office2010ColorTable.DataTimePickerDropDownSelectedDarkColor;
                }
                else if (this.mouseOver)
                {
                    color1 = office2010ColorTable.DataTimePickerDropDownHighLightLightColor;
                    color2 = office2010ColorTable.DataTimePickerDropDownHighLightDarkColor;
                }
                else if (this.Selected && !this.mouseOver)
                {
                    color1 = office2010ColorTable.DataTimePickerDropDownLightColor;
                    color2 = office2010ColorTable.DataTimePickerDropDownDarkColor;
                    if (this.Office2010Theme == Office2010Theme.Black)
                    {
                        m_arrowColor = Color.White;
                    }
                }
                else if (buttonState == ButtonState.Normal)
                {
                    color1 = office2010ColorTable.DataTimePickerDropDownLightColor;
                    color2 = office2010ColorTable.DataTimePickerDropDownDarkColor;
                }

                blend.Positions = new float[] { 0.0F, 0.45F, 0.45F + 0.001F, 1.0F };

                if (buttonState == ButtonState.Pushed)
                    blend.Factors = new float[] { 0.0F, 0.8F, 1.0F, 0.4F };
                else
                    blend.Factors = new float[] { 0.0F, 0.5F, 1.0F, 0.0F };

                Rectangle rect = this.ClientRectangle;
                rect.Width -= 3;
                rect.Height--;

                brush = new LinearGradientBrush(rect, color1, color2, LinearGradientMode.Vertical);
                brush.Blend = blend;

                return brush;
            }
            else
                return base.GetBackGroundBrush(buttonState);
        }

        protected override Pen GetBorderPen(ButtonState buttonState)
        {
            if (this.Style == VisualStyle.Office2007)
            {
                Color color = Color.Empty;

                if (buttonState == ButtonState.Pushed)
                {
                    color = office2007ColorTable.DataTimePickerSelectedBorderColor;
                }
                else if (this.mouseOver || this.Parent.Focused)
                {
                    color = office2007ColorTable.DataTimePickerHighLightedBorderColor;
                }
                else if (this.Selected && !this.mouseOver)
                {
                    color = office2007ColorTable.DataTimePickerBorderColor;
                }
                else if (buttonState == ButtonState.Normal)
                {
                    color = Color.White;
                }

                return new Pen(color);
            }
            else if (this.Style == VisualStyle.Office2010)
            {
                Color color = Color.Empty;

                if (buttonState == ButtonState.Pushed)
                {
                    color = office2010ColorTable.DataTimePickerSelectedBorderColor;
                }
                else if (this.mouseOver || this.Parent.Focused)
                {
                    color = office2010ColorTable.DataTimePickerHighLightedBorderColor;
                }
                else if (this.Selected && !this.mouseOver)
                {
                    color = office2010ColorTable.DataTimePickerBorderColor;
                }
                else if (buttonState == ButtonState.Normal)
                {
                    color = office2010ColorTable.DataTimePickerBorderColor;
                }

                return new Pen(color);
            }
            else
                return base.GetBorderPen(buttonState);
        }

        /// <summary>
        /// Draws the styled control.
        /// </summary>
        /// <param name="g">Graphics object</param>
        /// <param name="buttonState">State of the button.</param>
        /// <param name="checkState">State of the checkbox.</param>
        /// <override/>
        protected override void DrawStyledControl(Graphics g, ButtonState buttonState, CheckState checkState)
        {
            bool bPrevMouseOver = this.mouseOver;

            if (this.Style != VisualStyle.Office2007 && this.Style != VisualStyle.Office2010)
                this.mouseOver = bPrevMouseOver || this.Selected;

            if (this.Dropped)
            {
                buttonState = ButtonState.Pushed;
            }

            base.DrawStyledControl(g, buttonState, checkState);

            this.mouseOver = bPrevMouseOver;
        }
    }

    /// <summary>
    /// Type Converter for SpecificCultureInfo. Retrieves only specific cultures.
    /// </summary>
    public class SpecificCultureInfoTypeConverter : CultureInfoConverter
    {
        /// <summary>
        /// Collection of standard values.
        /// </summary>
        private TypeConverter.StandardValuesCollection values;

        /// <summary>
        /// Returns collection of standard values.
        /// </summary>
        /// <param name="context">ITypeDescriptor Context </param>
        /// <returns>Return stadard values </returns>
        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (null == this.values)
            {
                CultureInfo[] infos = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
                ArrayList listCultures = new ArrayList();
                listCultures.Add(CultureInfo.InvariantCulture);
                listCultures.AddRange(infos);

                this.values = new TypeConverter.StandardValuesCollection(listCultures.ToArray());
            }
            return this.values;
        }
    }

    /// <summary>
    /// Used for VS2003 designer auto-generated code for DateTimePickerAdv.
    /// </summary>
    public class DateTimePickerAdvDesignerSerializer
        : CodeDomSerializer
    {
        private const string DEF_CALENDAR = "Calendar";

        public override object Deserialize(IDesignerSerializationManager manager, object codeObject)
        {
            // This is how we associate the component with the serializer.
            CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.
                GetSerializer(typeof(DateTimePickerAdv).BaseType, typeof(CodeDomSerializer));

            // This is the simplest case, in which the class just calls the base class
            // to do the work.
            return baseClassSerializer.Deserialize(manager, codeObject);
        }

        public override object Serialize(IDesignerSerializationManager manager, object value)
        {
            if (value == null) return null;

            CodeDomSerializer cd;
            CodeStatementCollection codeObject;

            cd = manager.GetSerializer(typeof(DateTimePickerAdv).BaseType, typeof(CodeDomSerializer)) as CodeDomSerializer;
            object serializedObj = cd.Serialize(manager, value);

            codeObject = serializedObj as CodeStatementCollection;
            if (codeObject != null && codeObject.Count > 0)
            {
                codeObject = serializedObj as CodeStatementCollection;

                for (int i = 0, len = codeObject.Count; i < len; i++)
                {
                    CodeExpressionStatement statement = codeObject[i] as CodeExpressionStatement;
                    if (statement == null) continue;

                    CodeMethodInvokeExpression methodExpression = statement.Expression as CodeMethodInvokeExpression;
                    if (methodExpression == null) continue;

                    CodeCastExpression codeCastExpression = methodExpression.Method.TargetObject as CodeCastExpression;
                    if (codeCastExpression == null) continue;

                    CodeFieldReferenceExpression field = codeCastExpression.Expression as CodeFieldReferenceExpression;
                    if (field != null && field.FieldName.IndexOf(DEF_CALENDAR) > 0)
                    {
                        field.FieldName = string.Format("{0}.{1}", (value as DateTimePickerAdv).Name, DEF_CALENDAR);
                    }
                }
            }
            return codeObject;
        }
    }

    public enum InvalidDateProcessing
    {
        /// <summary>
        /// Invalid value is reverted to the last known valid value.
        /// </summary>
        Revert,

        /// <summary>
        /// Is value is invalid, the nearest valid value is set.
        /// </summary>
        SetNearest
    }

    /// <summary>
    /// Specifies that this control supports culture-dependent behavior.
    /// </summary>
    internal interface ISupportCulture
    {
        CultureInfo Culture
        {
            get;
            set;
        }
    }

    /// <summary>
    /// The DateTimePickerAdv control extends the Windows Forms DateTimePicker control and provides several important features. 
    /// Internationalization - The DateTimePickerAdv control is fully culture aware.
    /// Databinding - Supports DataBinding with a data source. Supports null value.
    /// Custom dropdown - The DateTimePickerAdv can display a custom drop down calendar.
    /// XP Themes - The DateTimePickerAdv control supports XP themes when applicable.
    /// </summary>
    [Designer(typeof(DateTimePickerAdvDesigner))]
    [DesignerSerializer(typeof(DateTimePickerAdvDesignerSerializer), typeof(CodeDomSerializer))]
    [DefaultEvent("ValueChanged")]
    [
    System.Drawing.ToolboxBitmap(typeof(DateTimePickerAdv), "ToolboxIcons.DateTimePickerExt.bmp"),
    Description("Represents advanced DateTimePicker control with Databinding, CustomDropDown, Internationalization.")
    ]
    public class DateTimePickerAdv :
        GradientPanel,
        ISupportInitialize,
        IThemedControl,
        IVisualStyle,
        ISupportCulture
    {
        #region constants

        private const int m_nControlWidth = 232;
        private const int m_nControlHeigth = 20;

        private const int m_nCheckBoxOffsetLeft = 3;
        private const int m_nMirroredHorizontalAdjustment = 5;

        private const int DEF_SAUDI_ARABIA_LCID = 1025;
        private int c_nMAX_HOUR_24 = 23;
        private int c_nMAX_HOUR_12 = 11;
        private int c_nMAX_MINUTE_SECOND = 59;
        private int c_nMAX_MONTH = 12;
        private int c_nMIN_MONTH = 1;
        private int c_nMAX_YEAR = 9998;
        private int c_nMIN_YEAR = 1753;
        private int c_nMAX_DAY = 31;
        private int c_nMIN_DAY = 1;
        private int c_nWEEK_DAYS = 7;
        private int c_nTWELVE_HOURS = 12;
        #endregion

        #region Internal Classes
        /// <summary>
        /// Modified version of MonthCalenderAdv class to fit the requirements of DateTimePickerAdv.
        /// </summary>
        public class MonthCalendarForDateTimePickerAdv : MonthCalendarAdv
        {
            #region Fields
            private DateTimePickerAdv parent = null;
            #endregion
            
            #region Constructor
            public MonthCalendarForDateTimePickerAdv(bool dtCalendar, DateTimePickerAdv dtPicker)
                : base(dtCalendar)
            {
                parent = dtPicker;
            }
            #endregion

            #region Properties

            /// <summary>
            /// Gets or sets the current value of the calendar.
            /// </summary>
            [Description("Indicates the current value of the calendar.")]
            [Category("Behavior")]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            [Editor(typeof(DateTimeEditorAdv), typeof(UITypeEditor))]
            public new DateTime Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    bool ctl = (Control.ModifierKeys & Keys.Control) != 0;
                    if (!ctl)
                        this.CurrentSelDates.Clear();
                    base.Value = value;
                }
            }

            #endregion

            #region Overrides
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                if (parent != null)
                {
                    parent.ProcessCmdKey(ref msg, keyData);
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            #endregion
        }

        /// <summary>
        /// Class used to simplify translation of calendar-dependent date and time to calendar-independent and vice-versa.
        /// </summary>
        internal class CultureSpecifiecDateTime
        {
            #region Class Private Members
            /// <summary>
            /// Calendar dependent year value.
            /// </summary>
            private int m_iYear;

            /// <summary>
            /// Calendar dependent month value.
            /// </summary>
            private int m_iMonth;

            /// <summary>
            /// Calendar dependent day value.
            /// </summary>
            private int m_iDay;

            /// <summary>
            /// Calendar dependent hour value.
            /// </summary>
            private int m_iHour;

            /// <summary>
            /// Calendar dependent minute value.
            /// </summary>
            private int m_iMinute;

            /// <summary>
            /// Calendar dependent second value.
            /// </summary>
            private int m_iSecond;

            /// <summary>
            /// Current calendar independent date and time settings.
            /// </summary>
            private DateTime m_datetime;

            /// <summary>
            /// Indicates whether m_datetime value is valid or it should be updated.
            /// </summary>
            private bool m_dateTimeValid;

            /// <summary>
            /// Currently used calendar.
            /// </summary>
            private Calendar m_calendar;
            #endregion

            #region Class Properties
            /// <summary>
            /// Gets or sets calendar-independent date and time.
            /// </summary>
            public DateTime DateTime
            {
                get
                {
                    if (!m_dateTimeValid)
                        UpdateDateTime();

                    return m_datetime;
                }
                set
                {
                    if (value != m_datetime)
                    {
                        m_datetime = value;
                        UpdateLocalizedDateTime();
                        m_dateTimeValid = true;
                    }
                    else
                    {
                        //Necessary, Unless Year,Month,Day and all the fields are defaulted to zero while initialized with default value.
                        UpdateLocalizedDateTime();
                        m_dateTimeValid = true;
                    }
                }
            }

            public int WeekOfYear
            {
                get
                {
                    return m_calendar.GetWeekOfYear(this.DateTime, DateTimeFormatInfo.CurrentInfo.CalendarWeekRule, DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);      
                }
            }

            /// <summary>
            /// Returns the week of the year that includes the date in the specified System.DateTime.
            /// </summary>
            /// <param name="calendarWeekRule">A System.Globalization.CalendarWeekRule value that defines a calendar week.</param>
            /// <param name="day">A System.DayOfWeek value that represents the first day of the week.</param>
            /// <returns>A positive integer that represents the week of the year that includes the date in the time parameter.</returns>
            public int GetWeekOfYear(CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
            {     
                return m_calendar.GetWeekOfYear(this.DateTime,calendarWeekRule,firstDayOfWeek);
            }

            public int GetWeekOfYearIso8601(CalendarWeekRule calendarWeekRule, DayOfWeek firstDayOfWeek)
            {
                calendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
                Calendar calendar = new GregorianCalendar();
                DayOfWeek dayOfWeek = calendar.GetDayOfWeek(this.DateTime);
                if (dayOfWeek >= System.DayOfWeek.Monday && dayOfWeek <= System.DayOfWeek.Wednesday)
                {
                    this.DateTime = this.DateTime.AddDays(3.0);
                }
                return m_calendar.GetWeekOfYear(this.DateTime,calendarWeekRule,firstDayOfWeek);
            }

            /// <summary>
            /// Gets the one-based index of the day of week.
            /// </summary>
            public int DayOfWeek
            {
                get
                {
                    return (int)m_calendar.GetDayOfWeek(DateTime);
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent year.
            /// </summary>
            public int Year
            {
                get
                {
                    return m_iYear;
                }
                set
                {
                    if (value != m_iYear)
                    {
                        m_iYear = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent month.
            /// </summary>
            public int Month
            {
                get
                {
                    return m_iMonth;
                }
                set
                {
                    if (value != m_iMonth)
                    {
                        m_iMonth = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent day.
            /// </summary>
            public int Day
            {
                get
                {
                    return m_iDay;
                }
                set
                {
                    if (value != m_iDay)
                    {
                        m_iDay = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent hour.
            /// </summary>
            public int Hour
            {
                get
                {
                    return m_iHour;
                }
                set
                {
                    if (value != m_iHour)
                    {
                        m_iHour = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent minute.
            /// </summary>
            public int Minute
            {
                get
                {
                    return m_iMinute;
                }
                set
                {
                    if (value != m_iMinute)
                    {
                        m_iMinute = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets or sets the calendar-dependent second.
            /// </summary>
            public int Second
            {
                get
                {
                    return m_iSecond;
                }
                set
                {
                    if (value != m_iSecond)
                    {
                        m_iSecond = value;
                        m_dateTimeValid = false;
                    }
                }
            }

            /// <summary>
            /// Gets the count of days in current month.
            /// </summary>
            public int DaysInMonth
            {
                get
                {
                    return m_calendar.GetDaysInMonth(Year, Month);
                }
            }
            #endregion

            #region Class Initialization
            /// <summary>
            /// Initializes a new instance of the CultureSpecifiecDateTime class.
            /// </summary>
            /// <param name="datetime">Calendar-independent date and time value.</param>
            /// <param name="calendar">Calendar to be used for translating date and time.</param>
            public CultureSpecifiecDateTime(DateTime datetime, Calendar calendar)
            {
                m_calendar = calendar;
                DateTime = datetime;
            }

            /// <summary>
            /// Initializes a new instance of the CultureSpecifiecDateTime class.
            /// </summary>
            /// <param name="year">Calendar-dependent year.</param>
            /// <param name="month">Calendar-dependent month.</param> 
            /// <param name="day">Calendar-dependent day of month.</param> 
            /// <param name="hour">Calendar-dependent hour.</param>
            /// <param name="minute">Calendar-dependent minute.</param>
            /// /// <param name="second">Calendar-dependent second.</param>
            /// <param name="calendar">Calendar to be used for translating date and time.</param>          
            public CultureSpecifiecDateTime(int year, int month, int day, int hour, int minute, int second, Calendar calendar)
            {
                m_calendar = calendar;
                m_iYear = year;
                m_iMonth = month;
                m_iDay = day;
                m_iHour = hour;
                m_iMinute = minute;
                m_iSecond = second;

                UpdateDateTime();
            }
            #endregion

            #region Class Helper Methods
            /// <summary>
            /// Updates calendar-independent setting using calendar-dependent settings.
            /// </summary>
            private void UpdateDateTime()
            {
                m_dateTimeValid = true;

                int iDays = m_calendar.GetDaysInMonth(m_iYear, m_iMonth);
                m_iDay = Math.Min(iDays, m_iDay);

                m_datetime = m_calendar.ToDateTime(m_iYear, m_iMonth, m_iDay, m_iHour, m_iMinute, m_iSecond, 0);
            }

            /// <summary>
            /// Updates calendar-dependent settings using calendar-independent settings.
            /// </summary>
            private void UpdateLocalizedDateTime()
            {
                m_iYear = m_calendar.GetYear(m_datetime);
                m_iMonth = m_calendar.GetMonth(m_datetime);
                m_iDay = m_calendar.GetDayOfMonth(m_datetime);
                m_iHour = m_calendar.GetHour(m_datetime);
                m_iMinute = m_calendar.GetMinute(m_datetime);
                m_iSecond = m_calendar.GetSecond(m_datetime);
            }
            #endregion
        }
        #endregion
        #region Variables

        private System.ComponentModel.IContainer components;

        /// <summary>
        /// The popup window containing the calendar.
        /// </summary>
        private CalendarPopup popupWindow;
        private CultureInfo culture = CultureInfo.InvariantCulture;
        private DTField selectedField = null;
        private Point offset = new Point(1, 1);
        private Hashtable fields = null;
        private DateTime dvalue = DateTime.Now;
        private DateTime designValue;
        private DateTime maxValue = DateTime.MaxValue;
        private DateTime minValue = DateTime.MinValue;
        private string format = "dd MMMM yyyy";
        private string customFormat = string.Empty;
        private string[] days = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private string[] abrevDays = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private string[] months = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private string[] abrevMonths = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private int maxDayWidth = 0;
        private int maxAbrevDayWidth = 0;
        private int maxMonthWidth = 0;
        private int maxDigitDayWidth = 0;
        private int maxDigitHourWidth = 0;
        private int maxDigitMinuteWidth = 0;
        private int maxDigitMonthWidth = 0;
        private int maxYearWidth = 0;
        private int maxDigitYearWidth = 0;
        private int maxTwoDigitYearWidth = 0;

        private DateTimePickerDropButton dropButton;
        private MonthCalendarForDateTimePickerAdv monthCalendar;
        private Syncfusion.Windows.Forms.ThemedCheckButton checkBox;
        private int maxAbrevMonthWidth = 0;
        private DateTimePickerFormat dTformat = DateTimePickerFormat.Long;
        private ThemedComboBoxDrawing comboDrawing = null;
        private DateTimePickerUpDownButton numericUD;
        private bool isNullDate = false;
        private string nullString = "No date is selected";
        private int digitYear = 1900;
        private PopupControlContainer customPopupWindow = null;
        private bool customPopup = false;
        private LeftRightAlignment dropDownAlign = LeftRightAlignment.Left;
        private bool useCurrentCulture;
        private ArrayList iCalendars = new ArrayList();
        private bool m_bShowCheckBox = true;
        private bool showDropButton = true;
        private bool showUpDown = false;
        private bool showUpDownOnFocus = false;
        private int spacing = 0;
        private bool autoForwarding = false;
        private bool tabForwarding = false;
        private bool flatButton = false;
        private IDateTimePickerAdvMenu contextMenu;
        private bool tabLeave = true;
        private bool copyFieldOnly = false;
        private bool themedControls = false;
        private DateTimePickerFormat clipboardFormat = DateTimePickerFormat.Long;
        private bool useEnhancedMenu = false;
        private DateTimePickerAdvCalendarStore cStore = new DateTimePickerAdvCalendarStore();
        private bool enableNullDate = true;
        private NullModeKeyReset nullModeKeyReset = NullModeKeyReset.ArrowKeys;
        private DateTime dateOnPopup = DateTime.MaxValue;
        private bool isNullDateOnPopup = false;
        private bool initializing = false;
        private bool changedByBinding = false;

        private bool readOnly = false;
        private bool readonlyvaluechange = true;

        private VisualStyle style = VisualStyle.Default;
		/// <summary>
		/// Gets bordercolor.
		/// </summary>
        private Color m_styleBorderColor = Color.FromArgb(209, 211, 212);

        private Office2007Theme m_office2007Theme = Office2007Theme.Blue;
        private Office2007Colors m_office2007ColorTable = null;
        private Office2010Theme m_office2010Theme = Office2010Theme.Blue;
        private Office2010Colors m_office2010ColorTable = null;
        private bool enableNullKeys = true;
        private InvalidDateProcessing m_invalidDateProcessing = InvalidDateProcessing.SetNearest;
        private string m_previousDateSeparator = "/";
        private bool resetSelectionOnFocus = false;
        private bool showDropDownOnNull = false;
		/// <summary>
		/// Gets the metrocolor.
		/// </summary>
        private Color m_metroColor = Color.FromArgb(22, 165, 220);
        #endregion
        #region Events
        /// <summary>
        /// Occurs when check-box's checked state is changed.
        /// </summary>
        [Description("Occurs when check-box's checked state is changed.")]
        public event EventHandler CheckBoxCheckedChanged;

        /// <summary>
        /// Occurs on <see cref="Calendar"/> popup.
        /// </summary>
        [Description("Occurs on Calendar popup.")]
        public event EventHandler OnPopup;

        /// <summary>
        /// Occurs when <see cref="Value"/> property is changed.
        /// </summary>
        [Category("Property Changed")]
        [Description("Occurs when Value property is changed.")]
        public event EventHandler ValueChanged;

        /// <summary>
        /// Occurs before <see cref="Calendar"/> popup.
        /// </summary>
        [Description("Occurs before Calendar popup.")]
        public event EventHandler BeforePopup;

        /// <summary>
        /// Occurs when <see cref="Calendar"/> popup is closed.
        /// </summary>
        [Description("Occurs when Calendar popup is closed.")]
        public event PopupClosedEventHandler PopupClosed;

        /// <summary>
        /// Occurs when <see cref="StretchDropDownImage"/> property is changed.
        /// </summary>
        [Description("Occurs when StretchDropDownImage property is changed.")]
        public event EventHandler StretchDropDownImageChanged;

        /// <summary>
        /// Occurs when <see cref="Calendar"/> popup is closed.
        /// </summary>
        [Description("Occurs when KeyPress.")]
        new public event KeyPressEventHandler KeyPress;

        /// <summary>
        /// Raises StretchDropDownImageChanged event.
        /// </summary>
        private void RaiseStretchDropDownImageChanged()
        {
            if (StretchDropDownImageChanged != null)
            {
                StretchDropDownImageChanged(this, EventArgs.Empty);
            }
        }
        protected virtual void OnStretchDropDownImageChanged()
        {
            this.dropButton.StretchImage = this.StretchDropDownImage;

            RaiseStretchDropDownImageChanged();
        }

        protected virtual void OnCheckBoxCheckedChanged(EventArgs e)
        {
            if (this.CheckBoxCheckedChanged != null)
            {
                this.CheckBoxCheckedChanged(this, e);
            }
        }
        protected virtual void OnOnPopup(EventArgs e)
        {
            if (OnPopup != null && !initializing)
            {
                OnPopup(this, e);
            }
        }
        protected virtual void OnPopupClosed(PopupClosedEventArgs e)
        {
            this.dropButton.Dropped = false;
            this.dropButton.Invalidate();

            if (PopupClosed != null && !initializing)
            {
                PopupClosed(this, e);
            }
        }
        protected virtual void OnBeforePopup(EventArgs e)
        {
            this.dropButton.Dropped = true;

            if (BeforePopup != null && !initializing)
            {
                BeforePopup(this, e);
            }
        }
        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null && !initializing)
            {
                ValueChanged(this, e);
            }
        }

        #region Delegates

        /// <summary>
        /// Delegate used for the NullButtonEvent of the IDateTimePickerAdvCalendar.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        public delegate void NullButtonEventHandler(object sender, System.EventArgs e);

        /// <summary>
        /// Delegate used for the SelectDateEvent of the IDateTimePickerAdvCalendar.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        public delegate void SelectDateEventHandler(object sender, System.EventArgs e);

        /// <summary>
        /// Delegate used for the DateChangedEvent of the IDateTimePickerAdvCalendar.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs that contains the event data.</param>
        public delegate void DateChangedEventHandler(object sender, System.EventArgs e);

        #endregion
        #endregion
        #region Properties
        /// <summary>
        /// Gets or sets the theme color of the DateTimePickerAdv
        /// </summary>
        [
        Browsable(true),
        Category("MetroColor"),
        RefreshProperties(RefreshProperties.Repaint),
        Description("Gets or sets the background color of the control.")
        ]
        public Color MetroColor
        {
            get { return m_metroColor; }
            set
            {
                m_metroColor = value;
                OnStyleChanged();
                Invalidate();
            }
        }
        private bool ShouldSerializeMetroColor()
        {
            return this.MetroColor != Color.Empty;
        }
        private void ResetMetroColor()
        {
            this.MetroColor = Color.Empty;
        }
        /// <summary>
        /// Gets or sets a value indicating whether the shortest name of the weekdays should be used. This property value have influence only under Framework 2.0 and higher.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Browsable(true)]
        [Description("Gets or sets value indicating whether the shortest name of the weekdays should be used.	This property value have influence only under Framework 2.0 and higher.")]
        public bool UseShortestDayNames
        {
            get
            {
                return Calendar.UseShortestDayNames;
            }
            set
            {
                Calendar.UseShortestDayNames = value;
            }
        }
        /// <summary>
        /// Get or Set of Skin Manager Interface.
        /// </summary>
        private string vStyle;
        string IVisualStyle.VisualTheme
        {
            get
            {
                return vStyle;
            }
            set
            {
                vStyle = value;
                switch (value)
                {
                    case "Office2007Blue":
                        Style = VisualStyle.Office2007;
                        Office2007Theme = Office2007Theme.Blue;
                        break;
                    case "Office2007Silver":
                        Style = VisualStyle.Office2007;
                        Office2007Theme = Office2007Theme.Silver;
                        break;
                    case "Office2007Black":
                        Style = VisualStyle.Office2007;
                        Office2007Theme = Office2007Theme.Black;
                        break;
                    case "Managed":
                        Style = VisualStyle.Office2007;
                        Office2007Theme = Office2007Theme.Managed;
                        break;
                    case "Default":
                        Style = VisualStyle.Default;
                        break;
                    case "Metro":
                        Style = VisualStyle.Metro;
                        break;
                    case "Office2003":
                        Style = VisualStyle.Office2003;
                        break;                   
                    case "Office2007Outlook":
                        Style = VisualStyle.Office2007Outlook;
                        break;
                    case "OfficeXP":
                        Style = VisualStyle.OfficeXP;
                        break;
                    case "VS2005":
                        Style = VisualStyle.VS2005;
                        break;
                    case "VS2010":
                        Style = VisualStyle.VS2010;
                        break;
                    case "Office2010Blue":
                        Style = VisualStyle.Office2010;
                        Office2010Theme = Office2010Theme.Blue;
                        break;
                    case "Office2010Silver":
                        Style = VisualStyle.Office2010;
                        Office2010Theme = Office2010Theme.Silver;
                        break;
                    case "Office2010Black":
                        Style = VisualStyle.Office2010;
                        Office2010Theme = Office2010Theme.Black;
                        break;
                }
            }
        }
        /// <summary>
        /// Gets currently used calendar.
        /// </summary>
        [Browsable(false)]
        public Calendar CurrentCalendar
        {
            get
            {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
                if (culture.LCID == DEF_SAUDI_ARABIA_LCID)
                {
                    if (m_umAlCuma != null)
                        return m_umAlCuma;

                    foreach (Calendar cal in culture.OptionalCalendars)
                    {
                        UmAlQuraCalendar saudi_arabia_cal = cal as UmAlQuraCalendar;

                        if (saudi_arabia_cal != null)
                        {
                            return m_umAlCuma = saudi_arabia_cal;
                        }
                    }
                }
#endif

                return culture.Calendar;
            }
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
        private UmAlQuraCalendar m_umAlCuma;
#endif

        /// <summary>
        /// Gets or sets the image for DropDown button.
        /// </summary>
        [Description("Specifies the image to be set for the DropDown button."), Category("Appearance")]
        public Image DropDownImage
        {
            get
            {
                return this.dropButton.Image;
            }
            set
            {
                if (value != this.dropButton.Image)
                {
                    this.dropButton.Image = value;
                }
            }
        }

        /// <summary>
        /// Indicate whether the image for DropDownButton 
        /// is stretched or shrunk to fit the size of the DropDownButton.
        /// </summary>
        private bool m_stretchDropDownImage = false;

        /// <summary>
        /// Gets or sets a value indicating whether the image for scroll button 
        /// is stretched or shrunk to fit the size of the scroll button.
        /// </summary>
        [
        DefaultValue(false),
        Description("Indicate whether the image for DropDownButton is stretched or shrunk to fit the size of the DropDownButton."),
        Category("Behavior")
        ]
        public bool StretchDropDownImage
        {
            get
            {
                return m_stretchDropDownImage;
            }
            set
            {
                if (value != m_stretchDropDownImage)
                {
                    m_stretchDropDownImage = value;
                    OnStretchDropDownImageChanged();
                }
            }
        }

        /// <summary>
        /// Gets the one-based index of the week of year.
        /// </summary>
        [Browsable(false)]
        public int WeekOfYear
        {
            get
            {
                CultureSpecifiecDateTime cultureDateTime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                return cultureDateTime.WeekOfYear;
            }
        }

        /// <summary>
        /// Gets the Popup window calendar.
        /// </summary>
        [Browsable(false)]
        public CalendarPopup PopupWindow
        {
            get
            {
                return popupWindow;
            }
        }

        /// <summary>
        ///  Gets or sets the fields of the DateTimePickerAdv.
        /// </summary>
        protected DTField[] FieldsArray
        {
            get
            {
                ArrayList fieldsArray = new ArrayList(this.fields.Values);

                return (DTField[])fieldsArray.ToArray(typeof(DTField));
            }
            set
            {
                this.fields = new Hashtable();

                int i = 0;

                foreach (DTField field in value)
                {
                    this.fields[field] = i++;
                }

                this.RefreshFields();
            }
        }

        /// <summary>
        ///  Gets or sets the selected field of the DateTimePickerAdv.
        /// </summary>
        protected DTField SelectedField
        {
            get
            {
                return this.selectedField;
            }
            set
            {
                if (selectedField != value)
                {
                    this.SelectField(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether calendar drop down should be shown when pressing ALT + down arrow key 
        /// when NULL date is set.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if show drop down when null date is set; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false), Description("Indicates whether calendar drop down should be shown when pressing ALT + down arrow key when NULL date is set.")]
        public bool ShowDropDownOnNull
        {
            get
            {
                return showDropDownOnNull;
            }
            set
            {
                if (showDropDownOnNull != value)
                {
                    showDropDownOnNull = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Backspace or Delete keys make the date null.
        /// </summary>
        [Description("Determines if Backspace or Delete keys make the date null.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool EnableNullKeys
        {
            get
            {
                return enableNullKeys;
            }
            set
            {
                if (enableNullKeys != value)
                {
                    enableNullKeys = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the none date button visible.
        /// </summary>
        /// <remarks>
        /// if <see cref="EnableNullDate"/> and <see cref="EnableNullKeys"/> are both set to false,
        /// NoneButton is hidden anyway.
        /// </remarks>
        [
            Browsable(true),
            Category("Behavior"),
            DefaultValue(true),
            Description("Specifies if the none date button will be visible.")
        ]
        public bool NoneButtonVisible
        {
            get
            {
                return this.cStore.NoneEnabled;
            }
            set
            {
                this.cStore.NoneEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the Office style of the Date Time Picker.
        /// </summary>
        [
        Description("Specifies the visual style of the DateTimePickerAdv."),
        Category("Appearance"),
        DefaultValue(Syncfusion.Windows.Forms.VisualStyle.Default),
        TypeConverter(typeof(DefaultVisualStyleEnumFilter)),
        ]
        public Syncfusion.Windows.Forms.VisualStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                if (style != value)
                {
                    style = value;

                    this.OnStyleChanged();
                }
            }
        }

        private void OnStyleChanged()
        {
            this.dropButton.Style = style;
            this.numericUD.Style = style;
            this.checkBox.Style = style;
            if(ThemesEnabled)
                this.UpdateStyles();
            else if (style == VisualStyle.Default)
            {
                this.dropButton.NormalColor = SystemColors.Control;
                this.Border3DStyle = Border3DStyle.Sunken;
                this.BorderStyle = BorderStyle.Fixed3D;
                this.StyleBorderColor = Color.Black;
                this.DockPadding.All = 0;
            }
            else
            {
                this.Border3DStyle = Border3DStyle.Flat;
                this.BorderStyle = BorderStyle.FixedSingle;
                this.DockPadding.All = -2;
                this.ThemedChildControls = false;

                if (style == VisualStyle.Office2007)
                {
                    m_office2007ColorTable = Office2007Colors.GetColorTable(m_office2007Theme);
                    this.StyleBorderColor = m_office2007ColorTable.DataTimePickerBorderColor;
                    this.dropButton.NormalColor = m_office2007ColorTable.DataTimePickerDropDownDarkColor;
                    this.dropButton.ArrowColor = m_office2007ColorTable.DataTimePickerDropDownArrowColor;
                    this.Calendar.Style = style;
                }
                else if (style == VisualStyle.Office2010)
                {
                    m_office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);
                    this.StyleBorderColor = m_office2010ColorTable.DataTimePickerBorderColor;
                    this.Calendar.Style = style;
                }
                else if (style == VisualStyle.Metro)
                {
                    this.BorderColor = Color.FromArgb(209,211,212);
                    this.Calendar.Style = style;
                    this.dropButton.NormalColor = MetroColor;
                    Color selColor = ControlPaint.Light(MetroColor);
                    this.dropButton.SelectedColor = selColor;
                    this.dropButton.PressedColor = MetroColor;
                    this.StyleBorderColor = m_styleBorderColor;
                    this.checkBox.Style = style;
                    this.checkBox.MetroColor = MetroColor;
                    this.dropButton.ArrowColor = Color.White;
                    this.PopupWindow.Width = this.Width;
                }
                else
                {
                    this.StyleBorderColor = SystemColors.ControlDark;
                    dropButton.FlatColor = this.BorderColorInternal;
                }
            }

            if (monthCalendar != null)
            {
                this.Calendar.Style = style;
                if (style == VisualStyle.Metro)
                {
                    this.Calendar.MetroColor = MetroColor;
                    this.BorderColor = m_styleBorderColor;
                    this.Calendar.HeaderStartColor = Color.White;
                    this.Calendar.HeaderEndColor = Color.White;
                }
                else
                {
                    this.CalendarTitleBackColor = this.Calendar.HeaderStartColor;
                    this.CalendarTitleForeColor = this.Calendar.HeadForeColor;
                    this.CalendarTrailingForeColor = this.Calendar.InactiveMonthColor;
                    this.CalendarMonthBackground = this.Calendar.GridBackColor;
                    this.CalendarForeColor = this.Calendar.ForeColor;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the Office2007 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets / sets a value indicating the Office2007 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2007Theme.Blue)
        ]
        public Office2007Theme Office2007Theme
        {
            get 
            { 
                return m_office2007Theme; 
            }
            set
            {
                if (m_office2007Theme != value)
                {
                    m_office2007Theme = value;

                    OnOffice2007ThemeChanged();
                }
            }
        }

        protected virtual void OnOffice2007ThemeChanged()
        {
            ApplyOffice2007ThemeToChildren();

            if (this.Style == VisualStyle.Office2007)
            {
                m_office2007ColorTable = Office2007Colors.GetColorTable(m_office2007Theme);

                ApplyOffice2007Theme();

                this.Invalidate();
            }
        }

        private void ApplyOffice2007Theme()
        {
            this.StyleBorderColor = m_office2007ColorTable.DataTimePickerBorderColor;
            this.CalendarTitleBackColor = this.Calendar.HeaderStartColor;
            this.CalendarTitleForeColor = this.Calendar.HeadForeColor;
            this.CalendarTrailingForeColor = this.Calendar.InactiveMonthColor;
            this.CalendarMonthBackground = this.Calendar.GridBackColor;
            this.CalendarForeColor = this.Calendar.ForeColor;
        }

        private void ApplyOffice2007ThemeToChildren()
        {
            this.monthCalendar.Office2007Theme = m_office2007Theme;
            this.dropButton.Office2007Theme = m_office2007Theme;
            this.numericUD.Office2007ColorScheme = m_office2007Theme;
            this.checkBox.Office2007Theme = m_office2007Theme;
        }
        /// <summary>
        ///  Gets or sets the Office2010 theme used for drawing the control.
        /// </summary>
        [
        Description("Gets / sets a value indicating the Office2010 theme used for drawing the control."),
        Category("Appearance"),
        DefaultValue(Office2010Theme.Blue)
        ]
        public Office2010Theme Office2010Theme
        {
            get 
            { 
                return m_office2010Theme; 
            }
            set
            {
                if (m_office2010Theme != value)
                {
                    m_office2010Theme = value;

                    OnOffice2010ThemeChanged();
                }
            }
        }

        protected virtual void OnOffice2010ThemeChanged()
        {
            ApplyOffice2010ThemeToChildren();

            if (this.Style == VisualStyle.Office2010)
            {
                m_office2010ColorTable = Office2010Colors.GetColorTable(m_office2010Theme);

                ApplyOffice2010Theme();

                this.Invalidate();
            }
        }

        private void ApplyOffice2010Theme()
        {
            this.StyleBorderColor = m_office2010ColorTable.DataTimePickerBorderColor;
            this.CalendarTitleBackColor = this.Calendar.HeaderStartColor;
            this.CalendarTitleForeColor = this.Calendar.HeadForeColor;
            this.CalendarTrailingForeColor = this.Calendar.InactiveMonthColor;
            this.CalendarMonthBackground = this.Calendar.GridBackColor;
            this.CalendarForeColor = this.Calendar.ForeColor;
        }

        private void ApplyOffice2010ThemeToChildren()
        {
            this.monthCalendar.Office2010Theme = m_office2010Theme;
            this.dropButton.Office2010Theme = m_office2010Theme;
            this.numericUD.Office2010ColorScheme = m_office2010Theme;
            this.checkBox.Office2010Theme = m_office2010Theme;
        }

        /// <summary>
        /// Gets or sets a value indicating whether toggles the read only state of the picker.
        /// </summary>
        [Description("Toggles the read only state of the picker.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get 
            {
                return readOnly; 
            }
            set
            {
                if (readOnly != value)
                {
                    readOnly = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the valueChange when control is readonly.
        /// </summary>
        [Description("Determines ValueChange when control is readonly")]
        [Category("Behavior")]
        [DefaultValue(true)]

        public bool ReadOnlyValueChange
        {
            get
            {
                return readonlyvaluechange;
            }

            set
            {
                readonlyvaluechange = value;
            }
        }

        /// <summary>
        /// Gets or sets the drop down backcolor in normal mode.
        /// If color is empty then uses the default color.
        /// </summary>
        [
        Description("Gets or sets drop down back color in normal mode. If color is empty then uses the default color."),
        Category("Appearance")
        ]
        public Color DropDownNormalColor
        {
            get
            {
                return this.dropButton.NormalColor;
            }
            set
            {
                this.dropButton.NormalColor = value;
            }
        }

        protected bool ShouldSerializeDropDownNormalColor()
        {
            return this.dropButton.NormalColor != Color.Empty;
        }

        protected void ResetDropDownNormalColor()
        {
            this.dropButton.NormalColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets the drop down backcolor in pressed mode.
        /// If color is empty then uses the default color.
        /// </summary>
        [
        Description("Gets or sets drop down back color in pressed mode. If color is empty then uses the default color."),
        Category("Appearance")
        ]
        public Color DropDownPressedColor
        {
            get
            {
                return this.dropButton.PressedColor;
            }
            set
            {
                this.dropButton.PressedColor = value;
            }
        }

        protected bool ShouldSerializeDropDownPressedColor()
        {
            return this.dropButton.PressedColor != Color.Empty;
        }

        protected void ResetDropDownPressedColor()
        {
            this.dropButton.PressedColor = Color.Empty;
        }

        /// <summary>
        /// Gets or sets the drop down backcolor in selected mode.
        /// If color is empty then uses the default color.
        /// </summary>
        [
        Description("Gets or sets drop down back color in selected mode. If color is empty then uses the default color."),
        Category("Appearance")
        ]
        public Color DropDownSelectedColor
        {
            get
            {
                return this.dropButton.SelectedColor;
            }
            set
            {
                this.dropButton.SelectedColor = value;
            }
        }

        protected bool ShouldSerializeDropDownSelectedColor()
        {
            return this.dropButton.SelectedColor != Color.Empty;
        }

        protected void ResetDropDownSelectedColor()
        {
            this.dropButton.SelectedColor = Color.Empty;
        }

        #region BorderColors
        /// <summary>
        /// Overrides Border Color property
        /// </summary>
        public override Color BorderColor
        {
            get
            {
                return base.BorderColor;
            }
            set
            {
                base.BorderColor = value;
            }
        }

        [Documentation.DocumentationExclude()]
        protected bool ShouldSerializeBorderColor()
        {
            return base.BorderColor != Color.Empty;
        }
        [Documentation.DocumentationExclude()]
        protected void ResetBorderColor()
        {
            base.BorderColor = Color.Empty;
        }

        protected override Color BorderColorInternal
        {
            get
            {
                Color color = base.BorderColorInternal;

                if (color.IsEmpty)
                {
                    color = this.StyleBorderColor;
                }

                return color;
            }
        }

        private Color StyleBorderColor
        {
            get
            {
                return m_styleBorderColor;
            }
            set
            {
                if (m_styleBorderColor != value)
                {
                    m_styleBorderColor = value;

                    if (this.BorderColor.IsEmpty)
                    {
                        OnBorderColorChangedInternal();

                        RedrawBorders();
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Gets are sets the Back color of the control. (overridden property)
        /// </summary>        
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Documentation.DocumentationExclude()]
        protected bool ShouldSerializeBackColor()
        {
            return this.BackColor != SystemColors.Window;
        }

        [Documentation.DocumentationExclude()]
        protected new void ResetBackColor()
        {
            this.BackColor = SystemColors.Window;
        }

        [DefaultValue(true)]
        public new bool TabStop
        {
            get 
            { 
                return base.TabStop; 
            }
            set
            {
                base.TabStop = value;
            }
        }

        /// <summary>
        ///  Gets or sets the keys  which will toggle off null date.
        /// </summary>
        [Description("Indicates what keys will toggle off null date.")]
        [Category("Behavior")]
        [DefaultValue(NullModeKeyReset.ArrowKeys)]
        public NullModeKeyReset NullModeKeyReset
        {
            get 
            {
                return nullModeKeyReset; 
            }
            set
            {
                nullModeKeyReset = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the calendar will size according to the widths of the days.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether the calendar will size according to the widths of the days.")]
        public bool CalendarSizeToFit
        {
            get 
            {
                return cStore.SizeToFit; 
            }
            set
            {
                if (cStore.SizeToFit != value)
                {
                    cStore.SizeToFit = value;
                    if (this.monthCalendar != null)
                    {
                        this.monthCalendar.SizeToFit = value;
                    }
                }
           }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the null date support is enabled. If set to false the DateTimePicker will always have a selected date.
        /// </summary>
        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("Indicates whether the null date support is enabled. If set to false the DateTimePicker will always have a selected date.")]
        public bool EnableNullDate
        {
            get 
            { 
                return enableNullDate; 
            }
            set
            {
                if (enableNullDate != value)
                {
                    if (!value)
                    {
                        IsNullDate = false;
                        if (this.monthCalendar != null)
                        {
                            this.monthCalendar.NoneButton.Visible = false;
                        }
                    }
                    else
                    {
                        if (this.monthCalendar != null)
                        {
                            this.monthCalendar.NoneButton.Visible = true;
                        }
                    }
                    enableNullDate = value;
                    this.cStore.EnableNullDate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the context menu will be a Syncfusion XPMenu.
        /// </summary>
        [Description("Indicates if the context menu will be a Syncfusion XPMenu.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseEnhancedMenu
        {
            get 
            { 
                return useEnhancedMenu; 
            }
            set
            {
                if (useEnhancedMenu != value)
                {
                    useEnhancedMenu = value;
                    if (value)
                    {
                        this.contextMenu = new DateTimePickerAdvMenuExt();
                    }
                    else
                    {
                        this.contextMenu = new DateTimePickerAdvMenu();
                    }
                    WireContextMenuEvents();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkbox, drop button, updown and calendar will be themed.
        /// </summary>
        [Description("Indicates if the checkbox, drop button ,updown and calendar will be themed.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ThemedChildControls
        {
            get
            {
                return themedControls;
            }
            set
            {
                if (themedControls != value)
                {
                    themedControls = value;
                    this.checkBox.ThemesEnabled = value;
                    this.dropButton.ThemesEnabled = value;

                    this.cStore.ThemedEnabledGrid = value;
                    this.cStore.ThemedEnabledScrollButtons = value;
                    if (this.monthCalendar != null)
                    {
                        this.monthCalendar.ThemedEnabledGrid = value;
                        this.monthCalendar.ThemedEnabledScrollButtons = value;
                    }
                    this.numericUD.ThemesEnabled = value;
                    this.dropButton.DefaultButtonState = value ? System.Windows.Forms.ButtonState.Flat : System.Windows.Forms.ButtonState.Normal;

                    UpdateStyles();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UpDown will have a VS like look.
        /// </summary>
        [Description("Indicates if the UpDown will have a VS like look.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool VSLikeUpDown
        {
            get 
            { 
                return numericUD.VSLikeButton;
            }
            set
            {
                this.numericUD.VSLikeButton = value;
            }
        }

        /// <summary>
        /// Gets or sets the format of the value of the picker when copying.
        /// </summary>
        [Description("Indicates the format of the value of the picker when copying.")]
        [Category("Behavior")]
        [DefaultValue(DateTimePickerFormat.Long)]
        public DateTimePickerFormat ClipboardFormat
        {
            get 
            { 
                return this.clipboardFormat;
            }
            set
            { 
                this.clipboardFormat = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether when copy or paste operation is done, only the selected field is taken into consideration.
        /// </summary>
        [Description("Indicates if when copy or paste operation is done, only the selected field is taken into consideration.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool CopyFieldsOnly
        {
            get  
            { 
                return copyFieldOnly; 
            }
            set 
            { 
                copyFieldOnly = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the focus will be moved off the control when there are no fields to tab through.
        /// </summary>
        /// <remarks>
        /// This is active only when the TabForwarding property is set to true.
        /// </remarks>
        [Description("Indicates if the focus will be moved off the control when there are no fields to tab through.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool TabLeave
        {
            get { return tabLeave; }
            set { tabLeave = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dropdown button will have a flat appearance.
        /// </summary>
        [Description("Indicates if the dropdown buttonwill have a flat appearance.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool FlatDropButton
        {
            get 
            {  
                return flatButton; 
            }
            set
            {
                if (flatButton != value)
                {
                    flatButton = value;
                    dropButton.DefaultButtonState = value ? ButtonState.Flat : ButtonState.Normal;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DateTimePicker will advance to the next field.
        /// </summary>
        [Description("When the Tab key gets pressed indicates if the DateTimePicker will advance to the next field.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool TabForwarding
        {
            get 
            { 
                return this.tabForwarding;
            }
            set 
            {  
                this.tabForwarding = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether after the completion of a field the DateTimePicker advances to the next field.
        /// </summary>
        [Description("Indicates if after the completion of a field the DateTimePicker advances to the next field.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool AutoForwarding
        {
            get 
            { 
                return this.autoForwarding; 
            }
            set
            { 
                this.autoForwarding = value; 
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the fields of the picker.
        /// </summary>
        [Description("Indicates the spacing between the fields of the picker.")]
        [Category("Appearance")]
        [DefaultValue(0)]
        public int Spacing
        {
            get 
            { 
                return spacing; 
            }
            set
            {
                if (spacing != value)
                {
                    spacing = value;
                    this.RefreshFields();
                }
           }
        }

        /// <summary>
        /// Gets or sets the size of the popup Calendar.
        /// </summary>
        [Category("Appearance")]
        [Description("Indicates the size of the popup Calendar.")]
        public Size CalendarSize
        {
            get 
            { 
                return this.cStore.PopupSize; 
            }
            set
            {
                this.cStore.PopupSize = value;
                if (this.popupWindow != null)
                {
                    this.popupWindow.Size = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarSize()
        {
            if (this.CalendarSize == new Size(208, 176))
                return false;
            else
                return true;
        }

        protected void ResetCalendarSize()
        {
            this.CalendarSize = new Size(208, 176);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current culture of the machine will be used. The culture will be set to the <see cref="System.Globalization.CultureInfo.CurrentCulture"/> culture.
        /// </summary>
        [Description("Indicates if the current culture of the machine will be used. It will be set to CultureInfo.CurrentCulture.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool UseCurrentCulture
        {
            get 
            { 
                return useCurrentCulture; 
            }
            set
            {
                if (useCurrentCulture != value)
                {
                    useCurrentCulture = value;

                    if (useCurrentCulture)
                        this.Culture = CultureInfo.CurrentCulture;
                }
            }
        }

        /// <summary>
        /// Gets or sets the trailing forecolor of the popup calendar.
        /// </summary>
        [Description("Indicates the trailing fore color of the popup calendar.")]
        [Category("Appearance")]
        public Color CalendarTrailingForeColor
        {
            get
            { 
                return cStore.TrailingForeColor; 
            }
            set
            {
                cStore.TrailingForeColor = value;
                if (this.monthCalendar != null)
                {
                    monthCalendar.InactiveMonthColor = value;
                }

                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).TrailingForeColor = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarTrailingForeColor()
        {
            if (this.CalendarTrailingForeColor == SystemColors.InactiveCaptionText)
                return false;
            else
                return true;
        }

        protected void ResetCalendarTrailingForeColor()
        {
            this.CalendarTrailingForeColor = SystemColors.InactiveCaptionText;
        }

        /// <summary>
        /// Gets or sets the title forecolor of the popup calendar.
        /// </summary>
        [Description("Indicates the title fore color of the popup calendar.")]
        [Category("Appearance")]
        public Color CalendarTitleForeColor
        {
            get 
            { 
                return cStore.TitleForeColor;
            }
            set
            {
                this.cStore.TitleForeColor = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.HeadForeColor = value;
                }

                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).TitleForeColor = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarTitleForeColor()
        {
            if (this.CalendarTitleForeColor == SystemColors.ActiveCaptionText)
                return false;
            else
                return true;
        }

        protected void ResetCalendarTitleForeColor()
        {
            this.CalendarTitleForeColor = SystemColors.ActiveCaptionText;
        }

        /// <summary>
        /// Gets or sets the title backcolor of the popup calendar.
        /// </summary>
        [Description("Indicates the title back color of the popup calendar.")]
        [Category("Appearance")]
        public Color CalendarTitleBackColor
        {
            get 
            { 
                return cStore.TitleBackColor; 
            }
            set
            {
                cStore.TitleBackColor = value;
                if (this.monthCalendar != null)
                {
                    monthCalendar.HeaderStartColor = value;
                }

                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).TitleBackColor = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarTitleBackColor()
        {
            if (this.CalendarTitleBackColor == SystemColors.ActiveCaption)
                return false;
            else
                return true;
        }

        protected void ResetCalendarTitleBackColor()
        {
            this.CalendarTitleBackColor = SystemColors.ActiveCaption;
        }

        /// <summary>
        /// Gets or sets the backcolor of the popup calendar.
        /// </summary>
        [Description("Indicates the back color of the popup calendar.")]
        [Category("Appearance")]
        public Color CalendarMonthBackground
        {
            get 
            { 
                return cStore.MonthBackground;
            }
            set
            {
                cStore.MonthBackground = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.GridBackColor = value;
                }
                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).CalendarMonthBackground = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarMonthBackground()
        {
            if (this.cStore.MonthBackground == SystemColors.Window)
                return false;
            else
                return true;
        }

        protected void ResetCalendarMonthBackground()
        {
            this.CalendarMonthBackground = SystemColors.Window;
        }

        /// <summary>
        /// Gets or sets the forecolor of the popup calendar.
        /// </summary>
        [Description("Indicates the fore color of the popup calendar.")]
        [Category("Appearance")]
        public Color CalendarForeColor
        {
            get 
            { 
                return cStore.ForeColor;
            }
            set
            {
                cStore.ForeColor = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.ForeColor = value;
                }
                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).CalendarForeColor = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarForeColor()
        {
            if (this.cStore.ForeColor == this.ForeColor)
                return false;
            else
                return true;
        }

        protected void ResetCalendarForeColor()
        {
            this.CalendarForeColor = this.ForeColor;
        }

        /// <summary>
        /// Gets or sets the font of the popup calendar.
        /// </summary>
        [Description("Indicates the font of the popup calendar.")]
        [Category("Appearance")]
        public Font CalendarFont
        {
            get 
            { 
                return cStore.Font; 
            }
            set
            {
                cStore.Font = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.Font = value;
                }
                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).CalendarFont = value;
                }
            }
        }

        protected bool ShouldSerializeCalendarFont()
        {
            if (this.cStore.Font == this.Font)
                return false;
            else
                return true;
        }

        protected void ResetCalendarFont()
        {
            this.CalendarFont = this.Font;
        }

        /// <summary>
        ///  Gets the popup calendar.
        /// </summary>
        [Description("The popup calendar.")]
        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MonthCalendarForDateTimePickerAdv Calendar
        {
            get
            {
                if (monthCalendar == null)
                {
                    this.CreatePopupControls();

                    if (!initializing)
                        this.InitializePopup();
                }

                return this.monthCalendar;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checked state of the checkbox is set.
        /// </summary>
        [Description("Indicates the checked state of the check box.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        [Bindable(true)]
        public bool Checked
        {
            get 
            { 
                return checkBox.Checked; 
            }
            set 
            { 
                checkBox.Checked = value; 
                this.CheckBox_CheckedChanged(checkBox, EventArgs.Empty); 
            }
        }

        /// <summary>
        /// Gets or sets the image list of the popup menu of the popup calendar.
        /// </summary>
        [Description("Indicates the image list of the popup menu of the popup calendar.")]
        [Category("Appearance")]
        [DefaultValue(null)]
        public ImageList MonthImageList
        {
            get 
            { 
                return cStore.ImageList; 
            }
            set
            {
                cStore.ImageList = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.MonthImageList = value;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the size of the DateTimePickerAdv control.
        /// </summary>
        public new Size Size
        {
            get
            {
                return base.Size;
            }

            set
            {
                base.Size = value;
            }
        }

        protected bool ShouldSerializeSize()
        {
            if (this.Size.Width == m_nControlWidth && this.Size.Height == m_nControlHeigth)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets or sets the popup window`s alignment.
        /// </summary>
        [Description("Indicates the popup window`s allignment.")]
        [Category("Appearance")]
        public LeftRightAlignment DropDownAlign
        {
            get { return dropDownAlign; }
            set { dropDownAlign = value; }
        }

        protected bool ShouldSerializeDropDownAlign()
        {
            if (this.DropDownAlign == LeftRightAlignment.Left)
                return false;
            else
                return true;
        }

        protected void ResetDropDownAlign()
        {
            this.DropDownAlign = LeftRightAlignment.Left;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CustomPopupWindow will be shown instead of the internal calendar popup.
        /// </summary>
        [Description("Indicates if the CustomPopupWindow will be shown instead of the internal calendar popup.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool CustomDrop
        {
            get 
            { 
                return customPopup;
            }
            set
            {
                customPopup = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="PopupControlContainer"/> that will be shown instead of the internal calendar. 
        /// If the container contains controls implementing the <see cref="IDateTimePickerAdvCalendar"/> interface and the Active property set to true the 
        /// picker will interact with those controls like the internal calendar.
        /// </summary>
        [Description("Indicates the PopupControlContainer that will be shown instead of the internal calendar.")]
        [Category("Behavior")]
        [DefaultValue(null)]
        public PopupControlContainer CustomPopupWindow
        {
            get 
            { 
                return customPopupWindow; 
            }
            set
            {
                if (value == null)
                {
                    customPopup = false;
                    customPopupWindow = null;
                    return;
                }
                this.customPopupWindow = value;
                WireCalendarEvents();
            }
        }

        private void WireContextMenuEvents()
        {
            if (contextMenu != null)
            {
                this.contextMenu.Popup += new System.EventHandler(this.ContextMenu_Popup);
                this.contextMenu.Cut += new EventHandler(this.CutMI_Click);
                this.contextMenu.Copy += new EventHandler(this.CopyMI_Click);
                this.contextMenu.Paste += new EventHandler(this.PasteMI_Click);
                this.contextMenu.NoDateTime += new EventHandler(this.NoDateTimeMI_Click);
            }
        }

        /// <summary>
        /// Returns the Custom Popup Window with the calendar and wires the calendar events.
        /// This is kept in a separate method so that it can be invoked from ISupportInitialize.EndInit().
        /// </summary>
        private void WireCalendarEvents()
        {
            if (this.customPopupWindow != null)
            {
                customPopupWindow.ParentControl = this;
                customPopupWindow.ControlAdded += new ControlEventHandler(CustomPopup_ControlAdded);
                customPopupWindow.BeforePopup += new CancelEventHandler(this.PopupWindow_BeforePopup);
                customPopupWindow.CloseUp += new Syncfusion.Windows.Forms.PopupClosedEventHandler(this.PopupWindow_CloseUp);
                iCalendars.Clear();
                for (int i = 0; i < customPopupWindow.Controls.Count; i++)
                {
                    Control control = customPopupWindow.Controls[i];

                    // Changing this to look for IDateTimePickerCalendarSource which will
                    // in turn get us the IDateTimePickerAdvCalendar - modified 11/07/02
                    // to prevent problem in VB.NET when both MonthCalendar and IDateTimePickerAdvCalendar
                    // implement that same properties
                    if (control is IDateTimePickerCalendarSource)
                    {
                        IDateTimePickerAdvCalendar iCalendar = ((IDateTimePickerCalendarSource)control).GetCalendar();

                        if (iCalendar.Active)
                        {
                            // IDateTimePickerAdvCalendar icontrol = control as IDateTimePickerAdvCalendar;
                            iCalendar.NullButtonDown += new NullButtonEventHandler(ICalendar_NoneButtonDown);
                            iCalendar.SelectDate += new SelectDateEventHandler(ICalendar_SelectDate);
                            iCalendar.DateChange += new DateChangedEventHandler(ICalendar_DateChanged);
                            iCalendar.Value = this.Value; // Added 02/26/03
                            iCalendars.Add(iCalendar);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the year that will complete the year fields when typing.
        /// </summary>
        [Description("Indicates the year that will complete the year fields when typing.")]
        [Category("Behavior")]
        [DefaultValue(1900)]
        public int DigitYear
        {
            get { return digitYear; }
            set { digitYear = value; }
        }

        /// <summary>
        /// Gets or sets the text to be displayed, when no date is selected.
        /// </summary>
        [Description("Indicates the text to be displayed, when no date is selected.")]
        [Category("Behavior")]
        [DefaultValue("No date is selected")]
        public string NullString
        {
            get 
            { 
                return nullString; 
            }
            set
            {
                if (nullString != value)
                {
                    nullString = value;
                    if (isNullDate) Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there is no date selected.
        /// </summary>
        [Description("Indicates if there is no date selected.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsNullDate
        {
            get 
            { 
                return isNullDate; 
            }
            set
            {
                if (readOnly && !this.changedByBinding && !this.readonlyvaluechange)
                    return;

                if (isNullDate != value && enableNullDate)
                {
                    isNullDate = value;
                    if (value && fields.Count > 0)
                    {
                        SelectedField = this.Fields(0);
                    }
                    this.OnValueChanged(EventArgs.Empty);
                    this.OnBindableValueChanged(EventArgs.Empty);
                    Invalidate(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets the format of the picker.
        /// </summary>
        [Description("Indicates the format of the picker.")]
        [Category("Appearance")]
        [DefaultValue(DateTimePickerFormat.Long)]
        public DateTimePickerFormat Format
        {
            get 
            { 
                return dTformat; 
            }
            set
            {
                if (dTformat == value) return;
                dTformat = value;

                if (!initializing)
                {
                    RefreshFormats();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UpDown buttons will be visible when it gets the focus.
        /// </summary>
        [Description("Indicates if the UpDown buttons will be visible when got focus.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowUpDownOnFocus
        {
            get 
            { 
                return showUpDownOnFocus;
            }
            set
            {
                showUpDownOnFocus = value;
                if (value)
                {
                    this.numericUD.Visible = false;
                }
                else
                {
                    if (showUpDown)
                    {
                        this.numericUD.Visible = true;
                    }
                    else
                    {
                        this.numericUD.Visible = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the drop button is visible.
        /// </summary>
        [Description("Indicates if the drop button is visible.")]
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool ShowDropButton
        {
            get 
            { 
                return showDropButton; 
            }
            set
            {
                if (showDropButton != value)
                {
                    showDropButton = value;
                }
                this.dropButton.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the UpDown buttons are visible.
        /// </summary>
        [Description("Indicates if the UpDown buttons are visible.")]
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool ShowUpDown
        {
            get 
            { 
                return showUpDown;
            }

            set
            {
                if (showUpDown != value)
                {
                    showUpDown = value;
                    numericUD.Visible = value;

                    if (this.RightToLeft == RightToLeft.Yes)
                    {
                        this.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the checkBox will be visible.
        /// </summary>
        [Description("Indicates if the checkBox will be visible.")]
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowCheckBox
        {
            get
            {
                return m_bShowCheckBox;
            }

            set
            {
                if (m_bShowCheckBox != value)
                {
                    m_bShowCheckBox = value;
                    OnShowCheckBoxChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum value that can be picked from the picker.
        /// </summary>
        [Description("Indicates the minimum value that can be picked from the picker.")]
        [Category("Behavior")]
        public DateTime MinValue
        {
            get 
            { 
                return minValue; 
            }
            set
            {
                if (minValue == value) return;

                if (maxValue < value)
                {
                    throw new ArgumentOutOfRangeException("MinValue", value, "value couldn't be greater then MaxValue: " + MaxValue.ToString());
                }

                minValue = value;
                this.cStore.MinDate = value;
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.MinValue = value;
                }
                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).MinDate = value;
                }
                if (dvalue < minValue)
                {
                    dvalue = minValue;
                    RefreshFields();
                }
            }
        }

        protected bool ShouldSerializeMinValue()
        {
            if (this.MinValue == new DateTime(1753, 1, 1, 0, 0, 0))
                return false;
            else
                return true;
        }

        protected void ResetMinValue()
        {
            this.MinValue = new DateTime(1753, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// Gets or sets the maximum value that can be picked from the picker.
        /// </summary>
        [Description("Indicates the maximum value that can be picked from the picker.")]
        [Category("Behavior")]
        public DateTime MaxValue
        {
            get 
            { 
                return maxValue; 
            }
            set
            {
                if (maxValue == value) return;

                if (minValue > value)
                {
                    throw new ArgumentOutOfRangeException("MaxValue", value, "value couldn't be less then MinValue: " + MinValue.ToString());
                }

                if (maxValue < minValue)
                {
                    maxValue = minValue = value;
                    Value = value;
                }
                else
                {
                    maxValue = value;
                    if (dvalue > maxValue)
                    {
                        Value = MaxValue;
                    }
                }
                if (this.monthCalendar != null)
                {
                    this.monthCalendar.MaxValue = value;
                }
                for (int i = 0; i < iCalendars.Count; i++)
                {
                    ((IDateTimePickerAdvCalendar)iCalendars[i]).MaxDate = value;
                }
                cStore.MaxDate = maxValue;
            }
        }

        protected bool ShouldSerializeMaxValue()
        {
            if (this.MaxValue == DateTime.MaxValue)
                return false;
            else
                return true;
        }

        protected void ResetMaxValue()
        {
            this.MaxValue = DateTime.MaxValue;
        }

        /// <summary>
        /// Gets or sets the selected date of the picker.
        /// </summary>
        [Description("Indicates the selected date of the picker.")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor(typeof(DateTimeEditorAdv), typeof(UITypeEditor))]
        public DateTime Value
        {
            get
            {
                return dvalue;
            }
            set
            {
                if (readOnly && !this.changedByBinding && !this.readonlyvaluechange)
                    return;

                if (dvalue != value || this.IsNullDate == true)
                {
                    if (this.monthCalendar != null)
                    {
                        if (!initializing && !this.monthCalendar.IsInitializing)
                        {
                            this.IsNullDate = false;
                        }
                    }
                    else
                    {
                        if (!initializing)
                            this.IsNullDate = false;
                    }

                    dvalue = value;

                    if (this.selectedField == null ||
                        (this.selectedField.Definition.Type != FieldType.Year || !this.selectedField.M_bSkipYearCheck))
                    {
                        dvalue = CoerceValue(dvalue);
                        OnValueChanged(EventArgs.Empty);
                        OnBindableValueChanged(EventArgs.Empty);
                        RefreshFields();
                        if (monthCalendar != null)
                        {
                            monthCalendar.Value = dvalue;
                        }
                    }
                }
            }
        }

        protected bool ShouldSerializeValue()
        {
            return this.dvalue.Year != this.designValue.Year || this.dvalue.Month != this.designValue.Month || this.dvalue.Day != this.designValue.Day ||
                this.dvalue.Hour != this.designValue.Hour || this.dvalue.Minute != this.designValue.Minute || this.dvalue.Second != this.designValue.Second;
        }
        protected void ResetValue()
        {
            this.Value = this.designValue = DateTime.Now;
        }

        /// <summary>
        /// Occurs when the <see cref="BindableValue"/> property is changed.
        /// </summary>
        [
        Category(@"Property Changed"),
        Description(@"Occurs when the BindableValue property is changed.")
        ]
        public event EventHandler BindableValueChanged;

        /// <summary>
        /// Raises the <see cref="BindableValueChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="System.EventArgs" /> that contains the event data.</param>
        protected virtual void OnBindableValueChanged(EventArgs e)
        {
            if (BindableValueChanged != null)
                BindableValueChanged(this, e);
        }

        /// <summary>
        ///  Gets or sets the  wrapper property around the selected date of the picker. Use this property if you
        /// want to be able to set the value of the control to null.
        /// </summary>
        [Description("Wrapper property that indicates the selected date of the picker. Can be set to null.")]
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Bindable(true)]
        public object BindableValue
        {
            get
            {
                if (this.IsNullDate == true)
                    return null;
                else
                    return this.Value;
            }
            set
            {
                if (value == System.DBNull.Value || value == null)
                    this.IsNullDate = true;
                else
                {
                    bool assignable = typeof(DateTime).IsAssignableFrom(value.GetType());

                    if (assignable == false)
                        throw new ArgumentException();
                    else
                    {
                        if (this.IsNullDate == true)
                        {
                            this.IsNullDate = false;
                        }
                        this.changedByBinding = true;
                        this.Value = (DateTime)value;
                        this.changedByBinding = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the format of the picker when Format is Custom.
        /// </summary>
        [Description("Indicates the format of the picker when Format is Custom.")]
        [Category("Appearance")]
        public string CustomFormat
        {
            get 
            { 
                return customFormat; 
            }
            set
            {
                customFormat = value;
                if (Format == DateTimePickerFormat.Custom)
                {
                    format = value;
                    ParseCustomString(format);
                }
            }
        }

        protected bool ShouldSerializeCustomFormat()
        {
            if (this.CustomFormat == string.Empty)
                return false;
            else
                return true;
        }

        protected void ResetCustomFormat()
        {
            this.CustomFormat = string.Empty;
        }

        /// <summary>
        /// Gets or sets the current culture of the picker.
        /// </summary>
        [Description("Indicates the current culture of the picker.")]
        [Category("Appearance")]
        [TypeConverter(typeof(SpecificCultureInfoTypeConverter))]
        public CultureInfo Culture
        {
            get 
            { 
                if (this.UseCurrentCulture)
                {
                    CultureInfo currentCulture = new CultureInfo((int)NativeMethods.GetUserDefaultLCID(), false);
                    if (currentCulture != culture)
                        culture = currentCulture;
                }

                return culture;
            }
            set
            {
                if (culture != value)
                {
                    culture = value;
                    RefreshFormats();
                    RecalculateMaxWidths();
                    cStore.Culture = value;
                    if (this.monthCalendar != null)
                    {
                        monthCalendar.Culture = value;
                    }
                    m_previousDateSeparator = culture.DateTimeFormat.DateSeparator;
                }
            }
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
            base.OnHandleDestroyed(e);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            Office2007Colors.ManagedColorsApplied += new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            Office2010Colors.ManagedColorsApplied += new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
            RefreshFormats();
            RecalculateMaxWidths();
        }

        protected bool ShouldSerializeCulture()
        {
            if (this.UseCurrentCulture || this.Culture.Equals(new System.Globalization.CultureInfo(string.Empty)))
                return false;
            else
                return true;
        }

        protected void ResetCulture()
        {
            this.Culture = new System.Globalization.CultureInfo(string.Empty);
        }

        /// <summary>
        /// Gets or sets value indicating what action should be chosen if user try to enter value that is not within the specified min or max range.
        /// </summary>
        [DefaultValue(InvalidDateProcessing.SetNearest)]
        [Description("Gets or sets value indicating what action should be chosen if user try to enter value that is not within the specified min or max range.")]
        public InvalidDateProcessing InvalidDateProcessing
        {
            get
            {
                return m_invalidDateProcessing;
            }
            set
            {
                m_invalidDateProcessing = value;
            }
        }
        #endregion

        void ISupportInitialize.BeginInit()
        {
            this.initializing = true;
            this.designValue = dvalue = DateTime.Now;
        }
        void ISupportInitialize.EndInit()
        {
            // For Custom Popup
            WireCalendarEvents();

            for (int i = 0; i < iCalendars.Count; i++)
            {
                IDateTimePickerAdvCalendar icontrol = (IDateTimePickerAdvCalendar)iCalendars[i];
                icontrol.CalendarFont = this.CalendarFont;
                icontrol.CalendarForeColor = this.CalendarForeColor;
                icontrol.CalendarMonthBackground = this.CalendarMonthBackground;
                icontrol.Culture = this.Culture;
                icontrol.MaxDate = this.MaxValue;
                icontrol.MinDate = this.MinValue;
                icontrol.TitleBackColor = this.CalendarTitleBackColor;
                icontrol.TitleForeColor = this.CalendarTitleForeColor;
                icontrol.TrailingForeColor = this.CalendarTrailingForeColor;
                icontrol.Value = this.Value;
            }

            // Hide the panels if isNullDate
            Invalidate(true);

            this.DateTimePickerAdv_SizeChanged(this, EventArgs.Empty);
            this.initializing = false;

            this.dateOnPopup = this.Value;
            this.isNullDateOnPopup = this.IsNullDate;

            this.RefreshFormats();
        }

        public DateTimePickerAdv()
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(DateTimePickerAdv));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
            InitializeDateTimePickerAdv();
            InitializeCalendarStore();

            UpdateCheckBoxLocation();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);

            this.BorderColor = Color.Empty;

            this.fields = new Hashtable();

            if (XPThemes.IsThemedOS)
            {
                comboDrawing = new ThemedComboBoxDrawing(ThemedControls.COMBOBOX, this);
            }

            if (this.popupWindow == null || this.monthCalendar == null || !this.m_bInitializePopupCalled)
            {
                InitializePopup();
            }
                       
            SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
        }

        private void InitializeCalendarStore()
        {
            cStore.Font = Font;
            cStore.ForeColor = this.ForeColor;
            cStore.MonthBackground = SystemColors.Window;
            cStore.Size = new Size(1, 1);
            cStore.TitleBackColor = SystemColors.ActiveCaption;
            cStore.TitleForeColor = SystemColors.ActiveCaptionText;
            cStore.TrailingForeColor = SystemColors.InactiveCaptionText;
            cStore.Culture = CultureInfo.CurrentCulture;
            cStore.MaxDate = this.maxValue;
            cStore.MinDate = this.minValue;
            cStore.PopupSize = new Size(189, 176);
            cStore.NoneEnabled = true;
            cStore.EnableNullDate = this.enableNullDate;
        }

        /// <summary>
        /// Virtual function used to initialize the default popup window.
        /// </summary>
        /// <param name="defaultPopup">The default popup window <see cref="PopupControlContainer"/>.</param>
        protected virtual void InitDefaultPopup(CalendarPopup defaultPopup)
        {
            defaultPopup.ParentControl = this;
            defaultPopup.Calendar = this.monthCalendar;
        }

        private void CustomPopup_ControlAdded(object sender, ControlEventArgs e)
        {
            Control control = e.Control;
            if (control is IDateTimePickerAdvCalendar)
            {
                IDateTimePickerAdvCalendar icontrol = control as IDateTimePickerAdvCalendar;

                icontrol.NullButtonDown += new NullButtonEventHandler(ICalendar_NoneButtonDown);
                icontrol.SelectDate += new SelectDateEventHandler(ICalendar_SelectDate);
                icontrol.DateChange += new DateChangedEventHandler(ICalendar_DateChanged);
                iCalendars.Add(icontrol);
            }
        }

        private void RecalculateMaxWidths()
        {
            Graphics g = this.CreateGraphics();

            maxDayWidth = 0;
            maxAbrevDayWidth = 0;
            maxMonthWidth = 0;
            maxAbrevMonthWidth = 0;
            maxDigitYearWidth = 0;
            maxTwoDigitYearWidth = 0;

            days = culture.DateTimeFormat.DayNames;
            for (int i = 0; i < 7; i++)
            {
                maxDayWidth = Math.Max(maxDayWidth, g.MeasureString(days[i], Font).ToSize().Width);
            }

            abrevDays = culture.DateTimeFormat.AbbreviatedDayNames;
            for (int i = 0; i < 7; i++)
            {
                maxAbrevDayWidth = Math.Max(maxAbrevDayWidth, g.MeasureString(abrevDays[i], Font).ToSize().Width);
            }

            for (int i = 1; i <= 12; i++)
            {
                months[i] = culture.DateTimeFormat.MonthNames[i - 1];
                maxMonthWidth = Math.Max(maxMonthWidth, g.MeasureString(months[i], Font).ToSize().Width);
            }
            for (int i = 1; i <= 12; i++)
            {
                abrevMonths[i] = culture.DateTimeFormat.AbbreviatedMonthNames[i - 1];
                maxAbrevMonthWidth = Math.Max(maxAbrevMonthWidth, g.MeasureString(abrevMonths[i], Font).ToSize().Width);
            }

            maxDigitDayWidth = 0;
            for (int i = 0; i < 31; i++)
            {
                maxDigitDayWidth = Math.Max(this.maxDigitDayWidth, DTField.MeasureString(g, i.ToString(), Font) - 2);
            }

            maxDigitHourWidth = 0;
            for (int i = 0; i < 23; i++)
            {
                maxDigitHourWidth = Math.Max(this.maxDigitHourWidth, DTField.MeasureString(g, i.ToString(), Font) - 2);
            }

            maxDigitMinuteWidth = 0;
            for (int i = 0; i < 59; i++)
            {
                maxDigitMinuteWidth = Math.Max(this.maxDigitMinuteWidth, DTField.MeasureString(g, i.ToString(), Font) - 2);
            }

            maxDigitMonthWidth = 0;
            for (int i = 1; i < 12; i++)
            {
                maxDigitMonthWidth = Math.Max(this.maxDigitMonthWidth, DTField.MeasureString(g, i.ToString(), Font) - 2);
            }

            maxYearWidth = DTField.MeasureString(g, 9999.ToString(), Font);
            maxDigitYearWidth = DTField.MeasureString(g, 9.ToString(), Font);
            maxTwoDigitYearWidth = DTField.MeasureString(g, 99.ToString(), Font);

            g.Dispose();

            RefreshFields();
        }
        private DateTime IncrementYear(bool inc)
        {
            return CurrentCalendar.AddYears(dvalue, inc ? 1 : -1);
        }

        private DateTime IncrementMonth(bool inc)
        {
            int oldYear = dvalue.Year;

            DateTime dt = CurrentCalendar.AddMonths(dvalue, inc ? 1 : -1);

            if (oldYear != dt.Year)
            {
                if (inc)
                    dt = CurrentCalendar.AddYears(dt, -1);
                else
                    dt = CurrentCalendar.AddYears(dt, 1);
            }

            return dt;
        }

        private DateTime IncrementDay(bool inc)
        {
            DTField fieldDay = this.FindField(FieldType.Day);

            if (fieldDay.Definition.Type != FieldType.Day)
                return dvalue;

            CultureSpecifiecDateTime datetime =
                new CultureSpecifiecDateTime(dvalue, CurrentCalendar);

            if (inc)
            {
                datetime.Day++;

                if (datetime.Day > fieldDay.Maximum) datetime.Day = 1;
            }
            else
            {
                datetime.Day--;

                if (datetime.Day < 1) datetime.Day = fieldDay.Maximum;
            }

            return datetime.DateTime;
        }

        private DateTime IncrementHour(bool inc)
        {
            DateTime dtOld = dvalue.Date;

            DateTime dt = dvalue.AddHours(inc ? 1 : -1);

            // Preserve the date
            if (dtOld != dt.Date)
            {
                if (inc)
                    dt = dt.AddDays(-1);
                else
                    dt = dt.AddDays(1);
            }

            return dt;
        }

        private DateTime IncrementMinute(bool inc)
        {
            int hourOld = dvalue.Hour;

            DateTime dt = dvalue.AddMinutes(inc ? 1 : -1);

            // Preserve the hour.
            if (dt.Hour != hourOld)
            {
                if (inc)
                    dt = dt.AddHours(-1);
                else
                    dt = dt.AddHours(1);
            }

            return dt;
        }

        private DateTime IncrementSecond(bool inc)
        {
            int minOld = dvalue.Minute;

            DateTime dt = dvalue.AddSeconds(inc ? 1 : -1);

            // Preserve the minute.
            if (dt.Minute != minOld)
            {
                if (inc)
                    dt = dt.AddMinutes(-1);
                else
                    dt = dt.AddMinutes(1);
            }

            return dt;
        }

        private DateTime IncrementAMPM()
        {
            int hoursOffset = 12;

            if ((dvalue.Hour > 12) || (dvalue.Hour == 12))
            {
                hoursOffset = -12;
            }

            return new DateTime(dvalue.Year, dvalue.Month, dvalue.Day, dvalue.Hour + hoursOffset, dvalue.Minute, dvalue.Second);
        }
        private void IncrementAMPMChangeValue()
        {
            try
            {
                DateTime value = IncrementAMPM();

                if (CheckDate(value))
                    Value = value;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private byte[] keyState = new byte[256];
        private byte[] charBuffer = new byte [2];

        #region Overrides

        public override Size GetPreferredSize(Size proposedSize)
        {
            NativeMethods.RECT rc = new NativeMethods.RECT(0, 0, 2, 0);

            if (this.ShowCheckBox && this.checkBox != null)
            {
                Size szCheckBox = this.checkBox.GetPreferredSize(Size.Empty);

                rc.Width += m_nCheckBoxOffsetLeft + szCheckBox.Width + 2;
                rc.Height = szCheckBox.Height;
            }

            if (this.ShowDropButton && this.dropButton != null)
            {
                Size szDropDown = this.dropButton.GetPreferredSize(Size.Empty);

                rc.Width += this.dropButton.Width;

                if (rc.Height < szDropDown.Height)
                {
                    rc.Height = szDropDown.Height;
                }
            }

            if (this.ShowUpDown && this.numericUD != null)
            {
                Size szUpDown = this.numericUD.GetPreferredSize(Size.Empty);

                rc.Width += szUpDown.Width;

                if (rc.Height < szUpDown.Height)
                {
                    rc.Height = szUpDown.Height;
                }
            }

            for (int i = 0; i < fields.Count; i++)
            {
                DTField field = Fields(i);
                rc.Width += field.Width + spacing;
            }

            int fieldsHeight = offset.X * 2 + Font.Height + (int)Font.Size / 7;

            if (rc.Height < fieldsHeight)
            {
                rc.Height = fieldsHeight;
            }

            CreateParams cp = this.CreateParams;
            AdjustWindowRectEx(ref rc, cp.Style, false, cp.ExStyle);

            Size szMax = this.MaximumSize;

            if (szMax.Width > 0 && rc.Width > szMax.Width)
            {
                rc.Width = szMax.Width;
            }

            if (szMax.Height > 0 && rc.Height > szMax.Height)
            {
                rc.Height = szMax.Height;
            }

            Size szMin = this.MinimumSize;

            if (szMin.Width > 0 && rc.Width < szMin.Width)
            {
                rc.Width = szMin.Width;
            }

            if (szMin.Height > 0 && rc.Height < szMin.Height)
            {
                rc.Height = szMin.Height;
            }

            return new Size(rc.Width, rc.Height);
        }       

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            string key = keyData.ToString();
            if (KeyPress != null)
                KeyPress(this, new KeyPressEventArgs(key[0]));
            if (this.popupWindow == null)
            {
                InitializePopup();
            }
            if ((this.popupWindow != null && !this.popupWindow.IsShowing()) ||
                (this.customPopupWindow != null
                            && this.customPopupWindow.IsShowing()))
            {
                if (this.readOnly)
                    return true;
                if (isNullDate)
                {
                    bool processed = false;
                    switch (nullModeKeyReset)
                    {
                        case NullModeKeyReset.Any:
                            IsNullDate = false;
                            processed = true;
                            break;
                        case NullModeKeyReset.ArrowKeys:
                            {
                                if (!this.showDropDownOnNull)
                                {
                                    if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)
                                    {
                                        IsNullDate = false;
                                        processed = true;
                                    }
                                }
                                else
                                {
                                    processed = true;
                                }
                                break;
                            }
                        case NullModeKeyReset.NumericKeys:
                            string str = keyData.ToString();
                            if ((str.StartsWith("D") && str.Length == 2) || str.StartsWith("NumPad"))
                            {
                                if(this.resetSelectionOnFocus)
                                    this.selectedField = Fields(0);
                                IsNullDate = false;
                                processed = true;
                            }
                            break;
                    }
                    if (!processed)
                    {
                        // Delegate every thing else to the base class.
                        return base.ProcessCmdKey(ref msg, keyData);
                    }
                }

                bool b = Syncfusion.Runtime.InteropServices.NativeMethods.GetKeyboardState(this.keyState);

                if (b)
                {
                    string str = Encoding.Unicode.GetString(charBuffer);

                    if (this.selectedField != null)
                    {
                        int index = (int)this.fields[this.selectedField];
                        if (index >= 0 && index < fields.Count - 1)
                            if (Fields(index + 1).Definition.Type == FieldType.String && Fields(index + 1).Text.Length == 1)
                                if (str == Fields(index + 1).Text)
                                    NextField();
                    }
                }

                if (((keyData & Keys.KeyCode) == Keys.A) && this.selectedField != null && this.selectedField.Definition.Type == FieldType.AMPM && this.selectedField.Text == "PM")
                {
                    this.IncrementAMPMChangeValue();
                }
                if (((keyData & Keys.KeyCode) == Keys.P) && this.selectedField != null && this.selectedField.Definition.Type == FieldType.AMPM && this.selectedField.Text == "AM")
                {
                    this.IncrementAMPMChangeValue();
                }
                if (((keyData & Keys.KeyCode) == Keys.X) && Control.ModifierKeys == Keys.Control)
                {
                    Cut();
                }
                if (((keyData & Keys.KeyCode) == Keys.C) && Control.ModifierKeys == Keys.Control)
                {
                    Copy();
                }
                if (((keyData & Keys.KeyCode) == Keys.V) && Control.ModifierKeys == Keys.Control)
                {
                    Paste();
                }
                if (enableNullKeys && (((keyData & Keys.KeyCode) == Keys.D0 && Control.ModifierKeys == Keys.Control) || (keyData == Keys.Back) || (keyData == Keys.Delete)))  
                {
                    IsNullDate = true;
                }
                if ((keyData & Keys.KeyCode) == Keys.Down && Control.ModifierKeys == Keys.Alt)
                {
                    this.DropButton_MouseDown(dropButton, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0));
                    return true;
                }
                if (keyData == Keys.Space)
                {
                    if (this.ShowCheckBox)
                    {
                        checkBox.CheckState = checkBox.CheckState == CheckState.Unchecked ? CheckState.Checked : CheckState.Unchecked;
                    }
                    return true;
                }

                if (keyData == Keys.Escape)
                {
                    if (this.customPopup)
                    {
                        if (this.customPopupWindow != null
                            && this.customPopupWindow.IsShowing())
                        {
                            customPopupWindow.HidePopup(PopupCloseType.Canceled);
                        }
                    }
                    else
                    {
                        if (this.popupWindow != null && this.popupWindow.IsShowing())
                        {
                            this.popupWindow.HidePopup(PopupCloseType.Canceled);
                        }
                    }
                    return true;
                }
                if (keyData == Keys.Enter)
                {
                    if (!customPopup)
                    {
                        // Do this only if the popup is currently showing, otherwise forward Enter keys.
                        if (this.popupWindow != null && popupWindow.IsShowing())
                        {
                            Value = this.Calendar.Value;
                            popupWindow.HidePopup(PopupCloseType.Done);
                            return true;
                        }
                    }
                }
                if ((this.ShowCheckBox && checkBox.Checked) || (!this.ShowCheckBox))
                {
                    if ((keyData == Keys.Up || keyData == Keys.Down) && !isNullDate)
                    {
                        this.Focus();
                        bool inc = keyData == Keys.Up;
                        this.IncrementField(inc);
                        return true;
                    }
                    if (keyData == Keys.Left || keyData == Keys.Right)
                    {
                        this.Focus();
                        if (selectedField != null)
                        {
                            if ((keyData == Keys.Left && PreviousField()) || ((keyData == Keys.Right) && NextField()))
                            {
                                return true;
                            }
                        }
                    }

                    int val = -1;
                    switch (keyData)
                    {
                        case Keys.D0: val = 0; 
                            break;
                        case Keys.D1: val = 1; 
                            break;
                        case Keys.D2: val = 2; 
                            break;
                        case Keys.D3: val = 3; 
                            break;
                        case Keys.D4: val = 4; 
                            break;
                        case Keys.D5: val = 5; 
                            break;
                        case Keys.D6: val = 6; 
                            break;
                        case Keys.D7: val = 7; 
                            break;
                        case Keys.D8: val = 8; 
                            break;
                        case Keys.D9: val = 9; 
                            break;
                        case Keys.NumPad0: val = 0; 
                            break;
                        case Keys.NumPad1: val = 1; 
                            break;
                        case Keys.NumPad2: val = 2; 
                            break;
                        case Keys.NumPad3: val = 3; 
                            break;
                        case Keys.NumPad4: val = 4; 
                            break;
                        case Keys.NumPad5: val = 5; 
                            break;
                        case Keys.NumPad6: val = 6; 
                            break;
                        case Keys.NumPad7: val = 7; 
                            break;
                        case Keys.NumPad8: val = 8; 
                            break;
                        case Keys.NumPad9: val = 9; 
                            break;
                    }
                    if (val != -1)
                    {
                        if (selectedField != null && selectedField.Definition.Type != FieldType.Year)
                        {
                            if (selectedField.Definition.Type != FieldType.AMPM)
                            {
                                if (selectedField.ProcessNumberKey(val) && this.autoForwarding)
                                {
                                    this.ValidateField();
                                    this.NextField();
                                }
                                else
                                    if (!this.autoForwarding)
                                        this.ValidateField();
                            }
                        }
                        else
                        {
                            if (selectedField.ProcessNumberKeyYear(val))
                            {
                                ValidateField();

                                if (this.autoForwarding)
                                {
                                    this.NextField();
                                }
                            }
                        }

                        return true;
                    }
                }
                if (keyData == Keys.Tab && this.tabForwarding)
                {
                    if (this.NextField(true))
                        return true;
                    else NextField();
                }
                if ((keyData & Keys.KeyCode) == Keys.Tab && Control.ModifierKeys == Keys.Shift
                    && this.tabForwarding)
                {
                    if (this.PreviousField(true))
                        return true;
                }

                if (((this.ShowCheckBox && this.checkBox.Checked) || (!this.ShowCheckBox)) && ((keyData == Keys.Home) || (keyData == Keys.End)))
                {
                    ProcessKey(keyData);
                }
            }
            else
            {
                if (keyData == Keys.Escape)
                {
                    bool processed = false;
                    if (this.customPopup)
                    {
                        if (this.customPopupWindow != null
                            && this.customPopupWindow.IsShowing())
                        {
                            customPopupWindow.HidePopup(PopupCloseType.Canceled);
                            processed = true;
                        }
                    }
                    else
                    {
                        if (this.popupWindow != null && this.popupWindow.IsShowing())
                        {
                            this.popupWindow.HidePopup(PopupCloseType.Canceled);
                            processed = true;
                        }
                    }
                    if (processed)
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool IsInputChar(char charCode)
        {
            return true;
        }

        #endregion

        #region  ProcessCmdKey helper methods

        private void ProcessKey(Keys key)
        {
            switch (this.SelectedField.Definition.Type)
            {
                // Date processing
                case FieldType.Day:
                    ProcessDay(key);
                    break;
                case FieldType.Month:
                    ProcessMonth(key);
                    break;
                case FieldType.Year:
                    ProcessYear(key);
                    break;

                // Time processing
                case FieldType.Hour:
                    ProcessHour(key);
                    break;
                case FieldType.Minute:
                    ProcessMinute(key);
                    break;
                case FieldType.Seconds:
                    ProcessSecond(key);
                    break;
                case FieldType.AMPM:
                    ProcessAMPM(key);
                    break;
            }
        }

        #region AMPM field helper methods
        /// <summary>
        /// Processes AMPM field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessAMPM(Keys key)
        {
            if (key == Keys.Home)
                ProcessAM();
            else if (key == Keys.End)
                ProcessPM();
        }

        /// <summary>
        /// Sets AM.
        /// </summary>
        private void ProcessAM()
        {
            // Proceed only in PM case.
            if (this.Value.Hour > c_nTWELVE_HOURS)
            {
                this.Value = this.Value.AddHours(-c_nTWELVE_HOURS);
                Invalidate();
            }
        }

        /// <summary>
        /// Sets PM.
        /// </summary>
        private void ProcessPM()
        {
            // Proceed only in AM case.
            if (this.Value.Hour < c_nTWELVE_HOURS)
            {
                this.Value = this.Value.AddHours(c_nTWELVE_HOURS);
                Invalidate();
            }
        }

        #endregion

        #region year field helper methods
        /// <summary>
        /// Processes year field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessYear(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinYear();
            else if (key == Keys.End)
                ProcessMaxYear();
        }

        /// <summary>
        /// Sets max year value.
        /// </summary>
        private void ProcessMaxYear()
        {
            this.Value = new DateTime(c_nMAX_YEAR, c_nMAX_MONTH, c_nMAX_DAY);
            Invalidate();
        }

        /// <summary>
        /// Sets min year value.
        /// </summary>
        private void ProcessMinYear()
        {
            this.Value = new DateTime(c_nMIN_YEAR, c_nMIN_MONTH, c_nMIN_DAY);
            Invalidate();
        }

        #endregion

        #region month field helper methods
        /// <summary>
        /// Processes month field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessMonth(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinMonth();
            else if (key == Keys.End)
                ProcessMaxMonth();
        }

        /// <summary>
        /// Sets max month value.
        /// </summary>
        private void ProcessMaxMonth()
        {
            int nCurrentMonth = this.Value.Month;
            this.Value = this.Value.AddMonths(c_nMAX_MONTH - nCurrentMonth);
            Invalidate();
        }

        /// <summary>
        /// Sets min month value.
        /// </summary>
        private void ProcessMinMonth()
        {
            int nCurrentMonth = this.Value.Month;
            this.Value = this.Value.AddMonths(1 - nCurrentMonth);
            Invalidate();
        }

        #endregion

        #region day field helper methods
        /// <summary>
        /// Processes day field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessDay(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinDay();
            else if (key == Keys.End)
                ProcessMaxDay();
        }

        /// <summary>
        /// Sets max day value.
        /// </summary>
        private void ProcessMaxDay()
        {
            if ((this.selectedField.Definition.DayFormat == DayFormat.FullName)
                || (this.selectedField.Definition.DayFormat == DayFormat.AbbreviatedName))
            {
                DayOfWeek day = this.Value.DayOfWeek;
                int nDaysToAdd = 0;

                if (day != DayOfWeek.Sunday)
                    nDaysToAdd += c_nWEEK_DAYS - (int)day;

                this.Value = this.Value.AddDays(nDaysToAdd);
            }
            else
            {
                int nCurrentDay = this.Value.Day;
                this.Value = this.Value.AddDays(this.selectedField.Maximum - nCurrentDay);
            }

            Invalidate();
        }

        /// <summary>
        /// Sets min day value.
        /// </summary>
        private void ProcessMinDay()
        {
            if ((this.selectedField.Definition.DayFormat == DayFormat.FullName)
                || (this.selectedField.Definition.DayFormat == DayFormat.AbbreviatedName))
            {
                DayOfWeek dayWeekDay = this.Value.DayOfWeek;
                int nDaysToAdd = 0;

                if (dayWeekDay != DayOfWeek.Sunday)
                    nDaysToAdd = 1 - (int)dayWeekDay;
                else
                    nDaysToAdd = 1 - c_nWEEK_DAYS;

                this.Value = this.Value.AddDays(nDaysToAdd);
            }
            else
            {
                int nCurrentDay = this.Value.Day;
                this.Value = this.Value.AddDays(1 - nCurrentDay);
            }

            Invalidate();
        }

        #endregion

        #region second field helper methods
        /// <summary>
        /// Processes minute field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessSecond(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinSecond();
            else if (key == Keys.End)
                ProcessMaxSecond();
        }

        /// <summary>
        /// Sets max second value.
        /// </summary>
        private void ProcessMaxSecond()
        {
            int nCurrentSecond = this.Value.Second;
            this.Value = this.Value.AddSeconds(c_nMAX_MINUTE_SECOND - nCurrentSecond);
            Invalidate();
        }

        /// <summary>
        /// Sets min second value.
        /// </summary>
        private void ProcessMinSecond()
        {
            int nCurrentSecond = this.Value.Second;
            this.Value = this.Value.AddSeconds(-nCurrentSecond);
            Invalidate();
        }

        #endregion

        #region minute field helper methods
        /// <summary>
        /// Processes minute field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessMinute(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinMinute();
            else if (key == Keys.End)
                ProcessMaxMinute();
        }

        /// <summary>
        /// Sets max minute value.
        /// </summary>
        private void ProcessMaxMinute()
        {
            int nCurrentMinute = this.Value.Minute;
            this.Value = this.Value.AddMinutes(c_nMAX_MINUTE_SECOND - nCurrentMinute);
            Invalidate();
        }

        /// <summary>
        /// Sets min minute value.
        /// </summary>
        private void ProcessMinMinute()
        {
            int nCurrentMinute = this.Value.Minute;
            this.Value = this.Value.AddMinutes(-nCurrentMinute);
            Invalidate();
        }

        #endregion

        #region hour field helper methods
        /// <summary>
        /// Processes hour field.
        /// </summary>
        /// <param name="key">Key pressed</param>
        private void ProcessHour(Keys key)
        {
            if (key == Keys.Home)
                ProcessMinHour();
            else if (key == Keys.End)
                ProcessMaxHour();
        }

        /// <summary>
        /// Sets max hour.
        /// </summary>
        private void ProcessMaxHour()
        {
            int nCurrentHour = this.Value.Hour;

            switch (this.SelectedField.Definition.HourFormat)
            {
                case HourFormat.OneOrTwoDigit12:
                case HourFormat.TwoDigit12:
                    this.Value = this.Value.AddHours(c_nMAX_HOUR_12 - nCurrentHour);
                    break;
                case HourFormat.TwoDigit24:
                case HourFormat.OneOrTwoDigit24:
                    this.Value = this.Value.AddHours(c_nMAX_HOUR_24 - nCurrentHour);
                    break;
            }

            Invalidate();
        }

        /// <summary>
        /// Sets min hour.
        /// </summary>
        private void ProcessMinHour()
        {
            int nCurrentHour = this.Value.Hour;
            this.Value = this.Value.AddHours(-nCurrentHour);
            Invalidate();
        }

        #endregion
        #endregion

        private bool PreviousField()
        {
            return PreviousField(false);
        }
        private bool PreviousField(bool tabKey)
        {
            int index = selectedField != null ? (int)this.fields[selectedField] : -1;
            if (index >= 0)
            {
                while (--index >= 0)
                {
                    if (Fields(index).Definition.Type != FieldType.String)
                    {
                        SelectField(Fields(index));
                        break;
                    }
                }
                if (index < 0)
                {
                    if ((tabLeave && tabKey) || !JumpField()) return false;
                    index = fields.Count;
                    while (--index >= 0)
                    {
                        if (Fields(index).Definition.Type != FieldType.String)
                        {
                            SelectField(Fields(index));
                            break;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Override this method when you want to modify the default behavior of the DateTimePickerAdv 
        /// that jumps to select the first field when the last is selected and the right arrow is pressed and vice versa.
        /// </summary>
        /// <returns>A bool value that indicates whether to jump or not.</returns>
        protected virtual bool JumpField()
        {
            return true;
        }
        private bool NextField()
        {
            return NextField(false);
        }
        private bool NextField(bool tabKey)
        {
            int index = selectedField != null ? (int)this.fields[selectedField] : -1;
            if (index < fields.Count)
            {
                while (++index < fields.Count)
                {
                    if (Fields(index).Definition.Type != FieldType.String)
                    {
                        SelectField(Fields(index));
                        break;
                    }
                }
                if (index == fields.Count)
                {
                    if ((this.tabLeave && tabKey) || !JumpField()) return false;
                    index = -1;
                    while (++index < fields.Count)
                    {
                        if (Fields(index).Definition.Type != FieldType.String)
                        {
                            SelectField(Fields(index));
                            break;
                        }
                    }
                }
            }
            return true;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.DesignMode)
                return;

            Focus();

            if (!readOnly)
            {
                if (this.isNullDateOnPopup && this.IsNullDate)
                {
                    // Special case where we have to just cancel this.
                    this.HidePopup(PopupCloseType.Canceled);
                }
                else
                {
                    this.HidePopup(PopupCloseType.Done);
                }

                if (e.Button == MouseButtons.Left)
                {
                    foreach (DTField field in fields.Keys)
                    {
                        if (field.Bounds.Contains(e.X, e.Y))
                        {
                            this.SelectField(field);
                        }
                    }
                }
                else
                {
                    if (this.ContextMenu == null && this.contextMenu != null)
                    {
                        contextMenu.ShowPopup(this, this.PointToClient(Control.MousePosition));
                    }
                }
            }

            base.OnMouseDown(e);
        }        

        private void SelectField(DTField field)
        {
            if (field.Definition.Type == FieldType.String)
                return;

            if (selectedField != null)
            {
                this.selectedField.ResetInsertYearMode();

                this.ValidateField();
            }
            selectedField = field;
            selectedField.StartedTyping = false;

            RefreshFields();

            // Invalidate();
        }

        private void Field_TextChanged(object sender, EventArgs e)
        {
            RepositionFields();

            Invalidate();
        }

        private void AddField(FieldDefinition fd)
        {
            DTField field = new DTField(fd, this);
            field.TextChanged += new EventHandler(Field_TextChanged);

            this.fields.Add(field, fields.Count);
        }

        private void ParseCustomString(string str)
        {
            FieldDefinitionCollection parse = DateTimeCustomFormatParser.Parse(str);

            fields.Clear();
            selectedField = null;
            foreach (FieldDefinition fd in parse)
            {
                AddField(fd);
            }

            if (fields.Count > 0)
                NextField();

            RefreshFields();
        }

        protected DTField Fields(int index)
        {
            if (index >= 0 && index < fields.Count)
            {
                foreach (DictionaryEntry entry in this.fields)
                {
                    if (index == (int)entry.Value)
                    {
                        return (DTField)entry.Key;
                    }
                }
            }

            return new DTField(null, this);
        }

        private void RefreshFormats()
        {
#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
            if (minValue < CurrentCalendar.MinSupportedDateTime)
                minValue = CurrentCalendar.MinSupportedDateTime;

            if (maxValue > CurrentCalendar.MaxSupportedDateTime)
                maxValue = CurrentCalendar.MaxSupportedDateTime;
#endif

            string currentDateSeparator = this.Culture.DateTimeFormat.DateSeparator;
            string cultureDateSeparator = new CultureInfo(this.Culture.Name).DateTimeFormat.DateSeparator;

            switch (dTformat)
            {
                case DateTimePickerFormat.Long:
                    format = culture.DateTimeFormat.LongDatePattern;
                    format = format.Replace(cultureDateSeparator, currentDateSeparator);
                    m_previousDateSeparator = currentDateSeparator;
                    break;

                case DateTimePickerFormat.Short:
                    format = culture.DateTimeFormat.ShortDatePattern;
                    format = format.Replace(cultureDateSeparator, currentDateSeparator);
                    m_previousDateSeparator = currentDateSeparator;
                    break;

                case DateTimePickerFormat.Time:
                    format = culture.DateTimeFormat.LongTimePattern;
                    break;

                case DateTimePickerFormat.Custom:
                    format = customFormat;
                    break;
            }

            ParseCustomString(format);
        }
        private DTField FindField(FieldType ft)
        {
            for (int i = 0; i < fields.Count; i++)
            {
                DTField field = Fields(i);
                if (field.Definition.Type == ft)
                    return field;
            }
            return new DTField(null, this);
        }

        private void RepositionFields()
        {
            bool bIsMirrored = GetIsMirrored();

            int nOffsetX = 0;
            if (bIsMirrored)
            {
                nOffsetX = ClientRectangle.Right - offset.X - m_nMirroredHorizontalAdjustment;
                if (this.ShowCheckBox && checkBox != null)
                {
                    nOffsetX -= checkBox.Width + 2;
                }
            }
            else
            {
                nOffsetX = offset.X;
                if (this.ShowCheckBox && checkBox != null)
                {
                    nOffsetX += checkBox.Width + 2;
                }
            }

            for (int i = 0; i < fields.Count; i++)
            {
                DTField field = Fields(i);

                if (bIsMirrored)
                {
                    nOffsetX -= field.Width + spacing;
                }

                field.Location = new Point(nOffsetX, (ClientRectangle.Height - field.Height) / 2);

                if (!bIsMirrored)
                {
                    nOffsetX += field.Width + spacing;
                }
            }
        }

        /// <summary>
        /// Refreshes all date-time input fields of the control.
        /// </summary>
        [Description("Refreshes all date-time input fields of the control.")]
        public void RefreshFields()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                DTField field = Fields(i);
                FieldDefinition fd = field.Definition;
                int iMonthNumber = CurrentCalendar.GetMonth(dvalue);
                int iYearNumber = CurrentCalendar.GetYear(dvalue);
                int iDay = CurrentCalendar.GetDayOfMonth(dvalue);

                switch (fd.Type)
                {
                    case FieldType.Day:
                        {
                            int iDayOfWeek = (int)CurrentCalendar.GetDayOfWeek(dvalue);

                            switch (fd.DayFormat)
                            {
                                case DayFormat.OneOrTwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitDayWidth;
                                        field.Text = iDay.ToString();
                                        break;
                                    }
                                case DayFormat.TwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitDayWidth;
                                        field.Text = string.Format("{0:00}", iDay);
                                        break;
                                    }
                                case DayFormat.AbbreviatedName:
                                    {
                                        field.MaxWidth = maxAbrevDayWidth;
                                        field.Text = abrevDays[iDayOfWeek];
                                        break;
                                    }
                                case DayFormat.FullName:
                                    {
                                        field.MaxWidth = maxDayWidth;
                                        field.Text = days[iDayOfWeek];
                                        break;
                                    }
                            }

                            field.Maximum = CurrentCalendar.GetDaysInMonth(iYearNumber, iMonthNumber);
                            break;
                        }
                    case FieldType.Hour:
                        {
                            switch (fd.HourFormat)
                            {
                                case HourFormat.OneOrTwoDigit12:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitHourWidth;
                                        int hr = dvalue.Hour;
                                        if (hr > 12)
                                        {
                                            hr = hr % 12;
                                        }
                                        if (hr == 0)
                                        {
                                            hr += 12;
                                        }

                                        field.Text = hr.ToString();
                                        field.Maximum = 12;
                                        break;
                                    }
                                case HourFormat.OneOrTwoDigit24:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitHourWidth;
                                        field.Text = dvalue.Hour.ToString();
                                        field.Maximum = 23;
                                        break;
                                    }
                                case HourFormat.TwoDigit12:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitHourWidth;
                                        int hr = dvalue.Hour;
                                        if (hr > 12)
                                        {
                                            hr = hr % 12;
                                        }
                                        if (hr == 0)
                                        {
                                            hr += 12;
                                        }
                                        field.Text = hr > 9 ? hr.ToString() : "0" + hr.ToString();
                                        field.Maximum = 12;
                                        break;
                                    }
                                case HourFormat.TwoDigit24:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitHourWidth;
                                        field.Text = dvalue.Hour.ToString().Length > 1 ? dvalue.Hour.ToString() : "0" + dvalue.Hour.ToString();
                                        field.Maximum = 23;
                                        break;
                                    }                               
                            }
                            break;
                        }
                    case FieldType.Minute:
                        {
                            switch (fd.MinuteFormat)
                            {     
                                case MinuteFormat.OneOrTwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMinuteWidth;
                                        field.Text = dvalue.Minute.ToString();
                                        break;
                                    }
                                case MinuteFormat.TwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMinuteWidth;
                                        field.Text = dvalue.Minute.ToString().Length > 1 ? dvalue.Minute.ToString() : "0" + dvalue.Minute.ToString();
                                        break;
                                    }                             
                            }
                            field.Maximum = 59;
                            break;
                        }
                    case FieldType.Month:
                        {
                            switch (fd.MonthFormat)
                            {
                                case MonthFormat.AbbreviatedName:
                                    {
                                        field.MaxWidth = maxAbrevMonthWidth;
                                        field.Text = abrevMonths[iMonthNumber];
                                        break;
                                    }
                                case MonthFormat.FullName:
                                    {
                                        field.MaxWidth = maxMonthWidth;
                                        field.Text = months[iMonthNumber];
                                        break;
                                    }
                                case MonthFormat.OneOrTwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMonthWidth;
                                        field.Text = iMonthNumber.ToString();
                                        break;
                                    }
                                case MonthFormat.TwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMonthWidth;
                                        field.Text = string.Format("{0:00}", iMonthNumber);
                                        break;
                                    }
                            }
                                 
                            field.Maximum = c_nMAX_MONTH;
                            break;
                        }
                    case FieldType.Seconds:
                        {
                            switch (fd.SecondsFormat)
                            {   
                                case SecondsFormat.OneOrTwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMinuteWidth;
                                        field.Text = dvalue.Second.ToString();
                                        break;
                                    }
                                case SecondsFormat.TwoDigit:
                                    {
                                        field.RightAlign = true;
                                        field.MaxWidth = this.maxDigitMinuteWidth;
                                        field.Text = dvalue.Second.ToString().Length > 1 ? dvalue.Second.ToString() : "0" + dvalue.Second.ToString();
                                        break;
                                    }
                            }
                            field.Maximum = 59;
                            break;
                        }
                    case FieldType.String:
                        {
                            field.Text = fd.StringData;
                            break;
                        }
                    case FieldType.Year:
                        {
                            if (this.selectedField == field && Focused)
                            {
                                field.RightAlign = true;
                                field.MaxWidth = this.maxYearWidth;
                                field.Text = iYearNumber.ToString();
                            }
                            else
                            {
                                switch (fd.YearFormat)
                                {
                                    case YearFormat.OneDigit:
                                        {
                                            field.RightAlign = true;
                                            field.MaxWidth = this.maxDigitYearWidth;
                                            field.Text = (iYearNumber % 10).ToString();
                                            this.DigitYear = iYearNumber - iYearNumber % 10;
                                            break;
                                        }
                                    case YearFormat.TwoDigit:
                                        {
                                            field.RightAlign = true;
                                            field.MaxWidth = this.maxTwoDigitYearWidth;
                                            field.Text = string.Format("{0:00}", (iYearNumber % 100));
                                            this.DigitYear = (iYearNumber / 100) * 100;
                                            break;
                                        }
                                    case YearFormat.Full:
                                        {
                                            field.RightAlign = true;
                                            field.MaxWidth = this.maxYearWidth;
                                            field.Text = iYearNumber.ToString();
                                            break;
                                        }
                                }
                            }
                            field.Maximum = CurrentCalendar.GetYear(MaxValue);
                            break;
                        }
                    case FieldType.AMPM:
                        {
                            bool pm = dvalue.Hour >= 12;
                            field.Text = pm
                                    ? culture.DateTimeFormat.PMDesignator
                                    : culture.DateTimeFormat.AMDesignator;
                            if (fd != null && fd.AMPMFormat == AMPMFormat.OneLetter)
                                if (field.Text == "PM")
                                    field.Text = "P";
                                else
                                    field.Text = "A";

                                    /* #region AMPMFormat
                                        AddDomain(new String[] {info.DateTimeFormat.AMDesignator,info.DateTimeFormat.PMDesignator},(Value.Hour<12)?0:1,fd);
                                        #endregion
                                    */
                            break;
                        }
                }
            }

            RepositionFields();

            Invalidate();
        }

        #region Validation
        private bool IsNumber(string text)
        {
            if (text.Length == 0) return false;
            for (int i = 0; i < text.Length; i++)
            {
                if (!char.IsNumber(text, i))
                {
                    return false;
                }
            }
            return true;
        }
        private DateTime ValidateYear()
        {
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }
            if ((value / 10) < 1)
            {
                value += digitYear - digitYear % 10;
            }
            if ((value / 100) < 1)
            {
                value += digitYear - digitYear % 100;
            }
            if ((value / 1000) < 1)
            {
                value += digitYear;//-digitYear % 1000;
            }
            if (value < this.minValue.Year || value > this.maxValue.Year)
            {
                value = value < this.minValue.Year ? minValue.Year : this.maxValue.Year;
            }

            try
            {
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Year = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }
        private DateTime ValidateMonth()
        {
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }

            try
            {
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Month = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }

        private DateTime ValidateDay()
        {
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }

            try
            {
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Day = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }
        private DateTime ValidateHour()
        {
            if (!this.selectedField.StartedTyping)
            {
                return dvalue;
            }
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }

            try
            {
                if (selectedField.Definition.HourFormat == HourFormat.OneOrTwoDigit12 || selectedField.Definition.HourFormat == HourFormat.TwoDigit12)
                {
                    if (dvalue.Hour >= 12 && value < 12)
                    {
                        value += 12;
                    }
                }
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Hour = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }
        private DateTime ValidateMinute()
        {
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }

            try
            {
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Minute = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }
        private DateTime ValidateSecond()
        {
            int value;
            if (IsNumber(selectedField.Text))
            {
                value = int.Parse(selectedField.Text);
            }
            else 
            { 
                return dvalue; 
            }

            try
            {
                CultureSpecifiecDateTime datetime = new CultureSpecifiecDateTime(dvalue, CurrentCalendar);
                datetime.Second = value;
                return datetime.DateTime;
            }
            catch
            {
                return dvalue;
            }
        }
        private void ValidateField()
        {
            if (this.selectedField == null || this.isNullDate)
                return;

            FieldType fieldType = selectedField.Definition.Type;
            if (fieldType == FieldType.AMPM)
                return;

            DateTime value;

            switch (fieldType)
            {
                case FieldType.Year:
                    value = ValidateYear();
                    break;
                case FieldType.Month:
                    value = ValidateMonth();
                    break;
                case FieldType.Day:
                    value = ValidateDay();
                    break;
                case FieldType.Hour:
                    value = ValidateHour();
                    break;
                case FieldType.Minute:
                    value = ValidateMinute();
                    break;
                case FieldType.Seconds:
                    value = ValidateSecond();
                    break;
                default:
                    throw new InvalidOperationException("Field validation can not be done: Field type is unknown to the validatino system.");
            }

            TrySetVelue(value);
        }

        /// <summary>
        /// Checks is value is within the min/max range and if so, sets the Value property to the specified value.
        /// </summary>
        /// <param name="value">Datetime value</param>
        private void TrySetVelue(DateTime value)
        {
            if (m_invalidDateProcessing == InvalidDateProcessing.SetNearest || CheckDate(value))
            {
                Value = value;
            }

            RefreshFields();
        }

        /// <summary>
        /// Checks whether the specified value is within the specified range.
        /// </summary>
        /// <param name="value">DateTime value to be checked.</param>
        /// <returns>True if the value is acceptible according to the min-max range.</returns>
        private bool CheckDate(DateTime value)
        {
            bool result = (value <= maxValue) && (value >= minValue);
            return result;
        }

        /// <summary>
        /// Proceedes with date coercing, based on mix-max dates range settings.
        /// </summary>
        /// <param name="value">DateTime value to be checked.</param>
        /// <returns>Returns the specified date without change if it is within the min-max dates range, or returns the closest allowed value.</returns>
        private DateTime CoerceValue(DateTime value)
        {
            if (value > maxValue)
                value = maxValue;

            if (value < minValue)
                value = minValue;

            return value;
        }
        #endregion
        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">Bool disposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SizeChanged -= new System.EventHandler(this.DateTimePickerAdv_SizeChanged);
                this.Enter -= new System.EventHandler(this.DateTimePickerAdv_Enter);
                this.FontChanged -= new System.EventHandler(this.DateTimePickerAdv_FontChanged);
                this.BackColorChanged -= new System.EventHandler(this.DateTimePickerAdv_BackColorChanged);
                this.Leave -= new System.EventHandler(this.DateTimePickerAdv_Leave);
                this.MouseEnter -= new EventHandler(this.DateTimePickerAdv_MouseEnter);
                this.MouseLeave -= new EventHandler(this.DateTimePickerAdv_MouseLeave);
                SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);

                if (components != null)
                    components.Dispose();

                if (contextMenu != null)
                {
                    this.contextMenu.Popup -= new System.EventHandler(this.ContextMenu_Popup);
                    this.contextMenu.Cut -= new EventHandler(this.CutMI_Click);
                    this.contextMenu.Copy -= new EventHandler(this.CopyMI_Click);
                    this.contextMenu.Paste -= new EventHandler(this.PasteMI_Click);
                    this.contextMenu.NoDateTime -= new EventHandler(this.NoDateTimeMI_Click);
                    this.contextMenu = null;
                }

                if (this.comboDrawing != null)
                {
                    comboDrawing.Dispose();
                    comboDrawing = null;
                }

                if (this.checkBox != null)
                {
                    this.checkBox.CheckStateChanged -= new System.EventHandler(this.CheckBox_CheckedChanged);
                    this.checkBox.Dispose();
                    this.checkBox = null;
                }

                if (this.numericUD != null)
                {
                    this.numericUD.UpDown -= new UpDownEventHandler(this.NumericUD_UpDown);
                    this.numericUD.Enter -= new EventHandler(this.NumericUD_Enter);
                    this.numericUD.Leave -= new EventHandler(this.NumericUD_Leave);
                    this.numericUD.MouseEnter -= new EventHandler(DateTimePickerAdv_MouseEnter);
                    this.numericUD.MouseLeave -= new EventHandler(DateTimePickerAdv_MouseLeave);
                    this.numericUD.Dispose();
                    this.numericUD = null;
                }

                if (this.dropButton != null)
                {
                    this.dropButton.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.DropButton_MouseDown);
                    this.dropButton.MouseEnter -= new EventHandler(DateTimePickerAdv_MouseEnter);
                    this.dropButton.MouseLeave -= new EventHandler(DateTimePickerAdv_MouseLeave);
                    this.dropButton.Dispose();
                    this.dropButton = null;
                }

                if (this.monthCalendar != null && !this.monthCalendar.IsDisposed)
                {
                    this.monthCalendar.DateSelected -= new System.EventHandler(this.MonthCalendar_DateSelected);
                    this.monthCalendar.NoneButtonClick -= new System.EventHandler(this.MonthCalendar_NoneButtonClick);
                    this.monthCalendar.DateChanged -= new System.EventHandler(this.MonthCalendar_DateChanged);
                    this.monthCalendar.SizeChanged -= new EventHandler(MonthCalendar_SizeChanged);
                    this.monthCalendar.Dispose();
                    this.monthCalendar = null;
                }

                if (this.popupWindow != null && !this.popupWindow.IsDisposed)
                {
                    this.popupWindow.BeforePopup -= new CancelEventHandler(this.PopupWindow_BeforePopup);
                    this.popupWindow.CloseUp -= new Syncfusion.Windows.Forms.PopupClosedEventHandler(this.PopupWindow_CloseUp);

                    this.popupWindow.Dispose();
                    this.popupWindow = null;
                }
                Office2010Colors.ManagedColorsApplied -= new Office2010Colors.ManagedColorsAppliedEventHandler(Office2010ManagedColorsApplied);
                Office2007Colors.ManagedColorsApplied -= new Office2007Colors.ManagedColorsAppliedEventHandler(Office2007ManagedColorsApplied);
            }

            base.Dispose(disposing);
        }

        private void CreatePopupControls()
        {
            this.popupWindow = new CalendarPopup();
            this.monthCalendar = new MonthCalendarForDateTimePickerAdv(true, this);
        }

        private bool m_bInitializePopupCalled = false;
        private void InitializePopup()
        {
            if (this.popupWindow == null || this.monthCalendar == null)
                this.CreatePopupControls();

            this.popupWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.monthCalendar).BeginInit();
      
            // popupWindow
            this.popupWindow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.popupWindow.Controls.AddRange(new System.Windows.Forms.Control[] { this.monthCalendar });
            this.popupWindow.Location = new System.Drawing.Point(32, 128);
            this.popupWindow.Name = "popupWindow";
            this.popupWindow.Size = cStore.PopupSize;
            this.popupWindow.TabIndex = 1;
            this.popupWindow.Visible = false;
            this.popupWindow.BeforePopup += new CancelEventHandler(this.PopupWindow_BeforePopup);
            this.popupWindow.CloseUp += new Syncfusion.Windows.Forms.PopupClosedEventHandler(this.PopupWindow_CloseUp);
     
            // monthCalendar
            this.monthCalendar.InvalidDateSelected += new EventHandler(MonthCalendar_InvalidDateSelected);
            this.monthCalendar.Dock = DockStyle.Fill;

            // this.monthCalendar.AllowSelection = Syncfusion.Windows.Forms.Grid.GridSelectionFlags.None;
            this.monthCalendar.AllowMultipleSelection = false;
            this.monthCalendar.Culture = cStore.Culture;
            this.monthCalendar.GridBackColor = cStore.MonthBackground;
            this.monthCalendar.HeaderStartColor = cStore.TitleBackColor;
            this.monthCalendar.HeadForeColor = cStore.TitleForeColor;
            this.monthCalendar.InactiveMonthColor = cStore.TrailingForeColor;
            this.monthCalendar.MaxValue = cStore.MaxDate;
            this.monthCalendar.MinValue = cStore.MinDate;
            this.monthCalendar.MonthImageList = cStore.ImageList;
            this.monthCalendar.Name = "monthCalendar";
            this.monthCalendar.ThemedEnabledGrid = cStore.ThemedEnabledGrid;
            this.monthCalendar.ThemedEnabledScrollButtons = cStore.ThemedEnabledScrollButtons;
            this.monthCalendar.Font = cStore.Font;
            
            // this.monthCalendar.Size = cStore.Size;
            this.monthCalendar.Value = this.dvalue;
            this.monthCalendar.SizeToFit = cStore.SizeToFit;

            this.monthCalendar.ScrollButtonSize = new System.Drawing.Size(17, 19);
            this.monthCalendar.Size = new System.Drawing.Size(206, 174);
            this.monthCalendar.TabIndex = 0;
            this.monthCalendar.ThemedEnabledGrid = this.ThemedChildControls;
            this.monthCalendar.ThemedEnabledScrollButtons = true;

            // this.monthCalendar.Value = new System.DateTime(2002, 12, 22, 17, 16, 25, 93);
            this.monthCalendar.VerticalAlignment = Syncfusion.Windows.Forms.Grid.GridVerticalAlignment.Middle;
            this.monthCalendar.DateSelected += new System.EventHandler(this.MonthCalendar_DateSelected);
            this.monthCalendar.NoneButtonClick += new System.EventHandler(this.MonthCalendar_NoneButtonClick);
            this.monthCalendar.DateChanged += new System.EventHandler(this.MonthCalendar_DateChanged);
            this.monthCalendar.SizeChanged += new EventHandler(MonthCalendar_SizeChanged);

            this.monthCalendar.Style = this.Style;
            Controls.Add(this.popupWindow);
            this.popupWindow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.monthCalendar).EndInit();

            InitDefaultPopup(popupWindow);

            m_bInitializePopupCalled = true;
        }

        private void InitTodayButton()
        {
            this.monthCalendar.TodayButton.Size = new System.Drawing.Size(134, 18);
            this.monthCalendar.TodayButton.Visible = true;
        }

        private void InitNoneButton()
        {
            this.monthCalendar.NoneButton.Location = new System.Drawing.Point(134, 0);
            this.monthCalendar.NoneButton.Size = new System.Drawing.Size(72, 18);
            this.monthCalendar.NoneButton.Visible = cStore.NoneEnabled && (cStore.EnableNullDate || enableNullKeys);
        }

        private void InitializeDateTimePickerAdv()
        {
            this.components = new System.ComponentModel.Container();
            this.dropButton = new DateTimePickerDropButton();
            this.checkBox = new Syncfusion.Windows.Forms.ThemedCheckButton();
            this.numericUD = new DateTimePickerUpDownButton();
            this.SuspendLayout();
            this.contextMenu = new DateTimePickerAdvMenu();
            WireContextMenuEvents();

            // dropButton
            this.dropButton.Dock = DockStyle.Right;
            this.dropButton.ButtonType = System.Windows.Forms.ScrollButton.Down;
            this.dropButton.CheckState = System.Windows.Forms.CheckState.Unchecked;
            this.dropButton.DefaultButtonState = System.Windows.Forms.ButtonState.Normal;
            this.dropButton.DrawText = false;

            // this.dropButton.Name = "dropButton";
            this.dropButton.Size = new System.Drawing.Size(18, 16);
            this.dropButton.Location = new System.Drawing.Point(209, 0);

            // this.dropButton.TabIndex = 0;
            this.dropButton.TabStop = false;
            this.dropButton.Text = "V";
            this.dropButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dropButton.ThemesEnabled = false;
            this.dropButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DropButton_MouseDown);

            this.dropButton.MouseEnter += new EventHandler(DateTimePickerAdv_MouseEnter);
            this.dropButton.MouseLeave += new EventHandler(DateTimePickerAdv_MouseLeave);

            UpdateDropButtonLocation();
 
            this.checkBox.Checked = true;
            this.checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox.DefaultButtonState = System.Windows.Forms.ButtonState.Normal;
            this.checkBox.DrawText = false;

            this.checkBox.Location = new System.Drawing.Point(m_nCheckBoxOffsetLeft, 1);

            // this.checkBox.Name = "checkBox";
            this.checkBox.Size = new System.Drawing.Size(13, 13);

            // this.checkBox.TabIndex = 2;
            this.checkBox.TabStop = false;
            this.checkBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox.ThemesEnabled = false;
            this.checkBox.CheckStateChanged += new System.EventHandler(this.CheckBox_CheckedChanged);

            // this.checkBox.MouseEnter += new EventHandler(DateTimePickerAdv_MouseEnter);
            // this.checkBox.MouseLeave += new EventHandler(DateTimePickerAdv_MouseLeave);
            UpdateCheckBoxLocation(); 

            // numericUD
            this.numericUD.Dock = DockStyle.Right;
            this.numericUD.Location = new System.Drawing.Point(211, 0);
            this.numericUD.Size = new System.Drawing.Size(16, 16);
            this.numericUD.Visible = false;
            this.numericUD.ScrollButtonAppearance = ScrollButtonAppearance.Vertical;
            this.numericUD.UpDown += new UpDownEventHandler(this.NumericUD_UpDown);
            this.numericUD.Enter += new EventHandler(this.NumericUD_Enter);
            this.numericUD.Leave += new EventHandler(this.NumericUD_Leave);
            this.numericUD.MouseEnter += new EventHandler(DateTimePickerAdv_MouseEnter);
            this.numericUD.MouseLeave += new EventHandler(DateTimePickerAdv_MouseLeave);
            this.numericUD.TabStop = false;

            UpdateUpDownLocation();
     
            // DateTimePickerAdv
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this.numericUD, this.checkBox, this.dropButton });

            // this.Name = "DateTimePickerAdv";
            this.Size = new System.Drawing.Size(m_nControlWidth, m_nControlHeigth);
            this.SizeChanged += new System.EventHandler(this.DateTimePickerAdv_SizeChanged);
            this.Enter += new System.EventHandler(this.DateTimePickerAdv_Enter);
            this.FontChanged += new System.EventHandler(this.DateTimePickerAdv_FontChanged);
            this.BackColorChanged += new System.EventHandler(this.DateTimePickerAdv_BackColorChanged);
            this.Leave += new System.EventHandler(this.DateTimePickerAdv_Leave);
            this.MouseEnter += new EventHandler(this.DateTimePickerAdv_MouseEnter);
            this.MouseLeave += new EventHandler(this.DateTimePickerAdv_MouseLeave);

            this.TabStop = true;

            if (this.DesignMode)
            {
                this.InitializePopup();
            }

            this.ResumeLayout(false);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Size = new System.Drawing.Size(150, 50);

        }
        #endregion

        private void NumericUD_Enter(object sender, EventArgs e)
        {
            this.Focus();
        }
        private void NumericUD_Leave(object sender, EventArgs e)
        {
            this.DateTimePickerAdv_Leave(this, e);
        }
        private void DropButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !dropButton.Visible)
                return;

            ValidateField();
            Point pt;

			if ( !this.Focused )
                this.Focus();

            if (popupWindow == null || this.monthCalendar == null || !m_bInitializePopupCalled)
            {
                OnBeforePopup(EventArgs.Empty);
                InitializePopup();
            }

            if (GetIsMirrored())
            {                                
                if (LeftRightAlignment.Right == dropDownAlign)
                {
                    pt = this.PointToScreen(new Point(0, Height));
                    pt.X += dropButton.Width;
                }
                else
                {
                    int nPosX = ClientRectangle.Width - offset.X - popupWindow.Width;
                    pt = this.PointToScreen(new Point(nPosX, Height));
                    if (checkBox.Visible)
                    {
                        pt.X -= checkBox.Width;
                    }
                }
            }
            else
            {
                if (dropDownAlign == LeftRightAlignment.Right)
                {
                    pt = this.PointToScreen(new Point(Width, Height));
                    pt.X -= popupWindow.Width + dropButton.Width + 2 * SystemInformation.Border3DSize.Width;
                }
                else
                {
                    pt = this.PointToScreen(new Point(offset.X, Height));
                    pt.X += checkBox.Visible ? checkBox.Width : 0;
                }
            }

            ShowPopup(pt);

            this.monthCalendar.RightToLeft = this.RightToLeft;
        }

        private void HidePopup(PopupCloseType type)
        {
            if (customPopup)
            {
                if (customPopupWindow.IsShowing())
                    this.customPopupWindow.HidePopup(type);
            }
            else
            {
                if (popupWindow != null && popupWindow.IsShowing())
                {
                    popupWindow.HidePopup(type);
                }
            }
        }
        private void ShowPopup(Point pt)
        {
            if (readOnly)
            {
                return;
            }

            if (customPopup && customPopupWindow != null)
            {
                if (!customPopupWindow.IsShowing())
                {
                    OnBeforePopup(EventArgs.Empty);
                    customPopupWindow.ShowPopup(pt);
                }
                else
                {
                    customPopupWindow.HidePopup(PopupCloseType.Canceled);
                }
            }
            else
            {
                if (popupWindow != null && !popupWindow.IsShowing())
                {
                    OnBeforePopup(EventArgs.Empty);
                }
                if (popupWindow == null || this.monthCalendar == null || !m_bInitializePopupCalled)
                {
                    OnBeforePopup(EventArgs.Empty);
                    InitializePopup();
                }

                if (!popupWindow.IsShowing())
                {
                    if (!this.monthCalendar.Visible)
                    {
                        this.monthCalendar.Visible = true;
                        this.monthCalendar.WrapText = true;
                    }

                    InitTodayButton();
                    InitNoneButton();

                    this.popupWindow.ShowPopup(pt);

                    this.monthCalendar.GridFocus();
                    OnOnPopup(EventArgs.Empty);
                }
                else
                {
                    popupWindow.HidePopup(PopupCloseType.Canceled);
                }
            }
        }

        private void PopupWindow_BeforePopup(object sender, CancelEventArgs e)
        {
            this.dateOnPopup = this.Value;
            this.isNullDateOnPopup = this.IsNullDate;
        }

        private void PopupWindow_CloseUp(object sender, Syncfusion.Windows.Forms.PopupClosedEventArgs args)
        {
            if (args.PopupCloseType == PopupCloseType.Canceled)
            {
                // User cancled the operation, so revert back to the old date-time.
                if (this.dateOnPopup != this.Value || this.isNullDateOnPopup != this.IsNullDate)
                {
                    this.Value = this.dateOnPopup;
                    this.IsNullDate = this.isNullDateOnPopup;
                }
            }
            else if (args.PopupCloseType == PopupCloseType.Done && this.monthCalendar != null)
            {
                // This is necessary only in this case:
                // 1) The current date is Null and the user dropsdown the calendar
                // 2) Calendar shows the Now date as the selected date (since not all calendars support Null date).
                // 3) User selects the already selected date, which doesn't fire the DateChanged event, for us to update the Value in the handler.
                // 4) So we have to update the Value here.
                this.Value = monthCalendar.Value;
            }
            this.Focus();
            OnPopupClosed(args);
        }

        /// <summary>
        /// Displays <see cref="Calendar"/> popup.
        /// </summary>
        [Description("Displays Calendar popup.")]
        public void DisplayCalendar()
        {
            this.DropButton_MouseDown(dropButton, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 0));
        }

        private void MonthCalendar_DateSelected(object sender, System.EventArgs e)
        {
            popupWindow.HidePopup(PopupCloseType.Done);
        }

        private void MonthCalendar_SizeChanged(object sender, System.EventArgs e)
        {
            if (cStore.SizeToFit)
            {
                if (this.popupWindow != null && this.monthCalendar != null)
                {
                    this.popupWindow.Size = new Size(this.monthCalendar.Width, cStore.PopupSize.Height);
                }
            }
        }
        private void MonthCalendar_DateChanged(object sender, System.EventArgs e)
        {
            if (!initializing && !this.monthCalendar.IsInitializing)
            {
                initializing = true;
                IsNullDate = false;
                initializing = false;
            }

            Value = monthCalendar.Value;

            if (!checkBox.Checked && this.Checked )
            {
                checkBox.Checked = true;
                this.CheckBox_CheckedChanged(checkBox, EventArgs.Empty);
            }   
        }

        private void ICalendar_NoneButtonDown(object sender, EventArgs e)
        {
            if (sender is IDateTimePickerAdvCalendar && ((IDateTimePickerAdvCalendar)sender).Active)
            {
                this.customPopupWindow.HidePopup(PopupCloseType.Canceled);
                IsNullDate = true;
            }
        }

        private void ICalendar_SelectDate(object sender, EventArgs e)
        {
            if (sender is IDateTimePickerAdvCalendar && ((IDateTimePickerAdvCalendar)sender).Active)
            {
                Value = ((IDateTimePickerAdvCalendar)sender).Value;
                this.customPopupWindow.HidePopup(PopupCloseType.Done);
            }
        }

        private void ICalendar_DateChanged(object sender, EventArgs e)
        {
            if (sender is IDateTimePickerAdvCalendar && ((IDateTimePickerAdvCalendar)sender).Active)
            {
                if (!initializing && !this.monthCalendar.IsInitializing)
                {
                    IsNullDate = false;
                }
                checkBox.Checked = true;
                Value = ((IDateTimePickerAdvCalendar)sender).Value;
                this.CheckBox_CheckedChanged(checkBox, EventArgs.Empty);
            }
        }

        private void OnShowCheckBoxChanged()
        {
            this.checkBox.Visible = this.ShowCheckBox;

            RefreshFields();

            Invalidate(true);
        }

        private void CheckBox_CheckedChanged(object sender, System.EventArgs e)
        {
            OnCheckBoxCheckedChanged(EventArgs.Empty);
            Invalidate();
        }

        private void IncrementField(bool inc)
        {
            if (selectedField != null)
            {
                this.selectedField.ResetInsertYearMode();

                try
                {
                    DateTime value;

                    switch (selectedField.Definition.Type)
                    {
                        case FieldType.Year:
                            value = IncrementYear(inc);
                            break;
                        case FieldType.Month:
                            value = IncrementMonth(inc);
                            break;
                        case FieldType.Day:
                            value = IncrementDay(inc);
                            break;
                        case FieldType.Hour:
                            value = IncrementHour(inc);
                            break;
                        case FieldType.Minute:
                            value = IncrementMinute(inc);
                            break;
                        case FieldType.Seconds:
                            value = IncrementSecond(inc);
                            break;
                        case FieldType.AMPM:
                            value = IncrementAMPM();
                            break;
                        default:
                            throw new InvalidOperationException("Field incrementation failed: invalid field type.");
                    }

                    TrySetVelue(value);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    this.RefreshFields();
                }
            }

            this.selectedField.StartedTyping = false;
        }
        private void NumericUD_UpDown(object sender, UpDownEventArgs e)
        {
            if (!this.Focused)
            {
                this.Focus();
            }
            string s;
            switch (selectedField.Definition.Type)
            {
                case FieldType.Day:
                    {
                        s = dvalue.Day.ToString();
                        break;
                    }
                case FieldType.Hour:
                    {
                        s = dvalue.Hour.ToString();
                        break;
                    }
                case FieldType.Minute:
                    {
                        s = dvalue.Minute.ToString();
                        break;
                    }
                case FieldType.Month:
                    {
                        s = dvalue.Month.ToString();
                        break;
                    }
                case FieldType.Seconds:
                    {
                        s = dvalue.Second.ToString();
                        break;
                    }
                case FieldType.Year:
                    {
                        s = dvalue.Year.ToString();
                        break;
                    }
                default:
                    {
                        s = string.Empty;
                        break;
                    }
            }

            if (selectedField.Text != s)
            {
                this.ValidateField();
            }
            this.IncrementField(e.ButtonID == 1);
        }

        protected override void OnThemeChanged(EventArgs e)
        {
            Invalidate(true);
            base.OnThemeChanged(e);
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x031A/*WM_THEMECHANGED*/)
            {
                this.RecreateHandle();
                if (this.comboDrawing != null)
                {
                    this.comboDrawing.RefreshThemeHandle();
                    this.Invalidate();
                }
                this.Invalidate(true);
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets or sets a value indicating whether selection should be reset to first field on focus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if reset selection on focus; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false), Description("Indicates whether the field selection should be reset on focus")]
        public bool ResetSelectionOnFocus
        {
            get 
            {
                return resetSelectionOnFocus;
            }
            set 
            {
                if (resetSelectionOnFocus != value)
                    resetSelectionOnFocus = value;
            }
        }
     
        protected override void OnGotFocus(EventArgs e)
        {
            if (resetSelectionOnFocus)
            {
                this.SelectField(Fields(0));
            }
            else
            {
                this.Invalidate();
            }
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (this.selectedField != null)
            {
                ValidateField();

                if (this.selectedField.M_bSkipYearCheck)
                {
                    this.selectedField.ResetInsertYearMode();
                    this.monthCalendar.Value = this.Value;
                }
            }
            base.OnLostFocus(e);
        }

        private void MonthCalendar_NoneButtonClick(object sender, System.EventArgs e)
        {
            IsNullDate = true;
            this.isNullDateOnPopup = true;
            this.popupWindow.HidePopup(PopupCloseType.Canceled);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_previousDateSeparator != this.Culture.DateTimeFormat.DateSeparator)
            {
                this.RefreshFormats();
            }

            base.OnPaint(e);

            if ((!(ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed)
                 || IgnoreThemeBackground) && (!Enabled || readOnly))
            {
                if (!ShouldSerializeBackColor())
                {
                    e.Graphics.FillRectangle(SystemBrushes.Control, ClientRectangle);
                }
            }
            else if (readOnly)
            {
                e.Graphics.Clear(SystemColors.Control);
            }
            else if (ThemesEnabled && XPThemes.IsThemedOS && XPThemes.IsThemeActive && XPThemes.IsAppThemed && !IgnoreThemeBackground)
            {
                using (Brush brush = new SolidBrush(SystemColors.Window))
                {
                    e.Graphics.FillRectangle(brush, ClientRectangle);
                }
            }

            SolidBrush foreBrush = new SolidBrush(ForeColor);

            if (isNullDate)
            {
                Size sz;
                if (nullString == string.Empty)
                {
                    sz = new Size(this.Width - 8, Font.Height);
                }
                else
                {
                    sz = e.Graphics.MeasureString(nullString, Font).ToSize();
                }

                int x, y = (ClientRectangle.Height - sz.Height) / 2;

                if (this.RightToLeft == RightToLeft.Yes)
                {
                    x = (dropButton.Visible ? dropButton.Width : 0) + (numericUD.Visible ? numericUD.Width : 0) + offset.X + 4;
                    x = this.Width - x - sz.Width;
                }
                else
                {
                    x = (checkBox.Visible ? checkBox.Width : 0) + offset.X + 4;
                }

                Point loc = new Point(x, y);

                if (Focused)
                {
                    Rectangle rc = new Rectangle(loc.X - 1, loc.Y, sz.Width + 1, sz.Height + 1);
                    ControlPaint.DrawFocusRectangle(e.Graphics, rc);
                }
                if (this.Enabled)
                {
                    e.Graphics.DrawString(nullString, Font, foreBrush, loc);
                }
                else
                {
                    Brush gray = new SolidBrush(SystemColors.GrayText);
                    e.Graphics.DrawString(nullString, Font, gray, loc);
                    gray.Dispose();
                }
            }
            else
            {
                string s = string.Empty;

                if (selectedField != null && Focused)
                {
                    Rectangle rc = selectedField.TextRectangle;
                    rc.Offset(2, 0);
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, rc);
                }

                for (int i = 0; i < fields.Count; i++)
                {
                    DTField field = Fields(i);
                    s += field.Text;

                    if ((this.ShowCheckBox && !this.checkBox.Checked) || !this.Enabled)
                    {
                        Brush grayBrush = new SolidBrush(SystemColors.GrayText);
                        e.Graphics.DrawString(field.Text, Font, grayBrush, new Rectangle(field.MiddleLocation, new Size(field.Width + 10, field.Height + 10)));
                        grayBrush.Dispose();
                    }
                    else
                    {
                        if (field == this.selectedField && this.Focused)
                        {
                            e.Graphics.DrawString(field.Text, Font, SystemBrushes.HighlightText, field.MiddleLocation);
                        }
                        else
                        {
                            e.Graphics.DrawString(field.Text, Font, foreBrush, field.MiddleLocation);
                        }
                    }
                }

                foreBrush.Dispose();
            }
        }

        private void DateTimePickerAdv_SizeChanged(object sender, System.EventArgs e)
        {
            UpdateCheckBoxLocation();

            RefreshFields();

            Invalidate(true);
        }

        private void DateTimePickerAdv_Leave(object sender, System.EventArgs e)
        {
            if (selectedField != null && ((ShowCheckBox && this.Checked) || (!ShowCheckBox)) && !isNullDate)
            {
                this.ValidateField();
            }
            if (showUpDownOnFocus)
            {
                this.numericUD.Visible = false;
            }

            if (this.Style == VisualStyle.OfficeXP)
            {
                this.StyleBorderColor = MenuColors.DropDownBorderColor;
            }
            else if (this.Style == VisualStyle.Office2003)
            {
                this.StyleBorderColor = MenuColors.DropDownBorderColor;
            }
            else if (this.Style == VisualStyle.Metro)
            {
                this.StyleBorderColor = m_styleBorderColor;
            }
            this.dropButton.Selected = false;
            this.dropButton.Invalidate();

            this.numericUD.Selected = false;
            this.numericUD.ButtonState = ButtonState.Normal;
            this.numericUD.Invalidate();

            this.checkBox.Selected = false;
            this.checkBox.Invalidate();

            RefreshFields();
        }

        private void DateTimePickerAdv_Enter(object sender, System.EventArgs e)
        {
            if (selectedField == null)
            {
                NextField();
            }
            if (showUpDownOnFocus)
            {
                this.numericUD.Visible = true;
            }

            if (this.Style == VisualStyle.OfficeXP)
            {
                this.StyleBorderColor = MenuColors.SelBorderColor;
            }
            else if (this.Style == VisualStyle.Office2003)
            {
                this.StyleBorderColor = Office2003Colors.SelBorderColor;
            }
            else if (this.Style == VisualStyle.Metro)
            {
                this.StyleBorderColor = m_styleBorderColor;
            }
            this.dropButton.Selected = true;
            this.dropButton.Invalidate();

            this.numericUD.Selected = true;
            this.numericUD.Invalidate();

            this.checkBox.Selected = true;
            this.checkBox.Invalidate();

            Invalidate();
        }

        private void DateTimePickerAdv_FontChanged(object sender, System.EventArgs e)
        {
            RecalculateMaxWidths();
            ParseCustomString(format);

            Size = new Size(Width, offset.X * 2 + Font.Height + (int)Font.Size / 7);
            if (Height < 20) Height = 20;
        }

        private void DateTimePickerAdv_BackColorChanged(object sender, System.EventArgs e)
        {
            this.checkBox.BackColor = this.BackColor;
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            // Swap location of checkBox
            UpdateDropButtonLocation();
            UpdateUpDownLocation();

            UpdateCheckBoxLocation();

            RefreshFields();
        }

        private void UpdateDropButtonLocation()
        {
            bool bIsMirrored = GetIsMirrored();

            dropButton.Dock = bIsMirrored ? DockStyle.Left : DockStyle.Right;
        }

        private void UpdateUpDownLocation()
        {
            bool bIsMirrored = GetIsMirrored();
            numericUD.Dock = bIsMirrored ? DockStyle.Left : DockStyle.Right;
        }

        private void UpdateCheckBoxLocation()
        {
            int nCheckBoxX = m_nCheckBoxOffsetLeft;
            int nCheckBoxY = (Height - checkBox.Height) / 2; // checkBox.Location.Y;
            if (GetIsMirrored())
            {
                nCheckBoxX = ClientRectangle.Width - checkBox.Size.Width - m_nCheckBoxOffsetLeft;
            }

            checkBox.Location = new Point(nCheckBoxX, nCheckBoxY);
        }

        /// <summary>
        /// Cuts <see cref="Value"/> property to clipboard.
        /// </summary>
        /// <returns>
        /// True if value has been sucessfully cut.
        /// </returns>
        public bool Cut()
        {
            if (!this.copyFieldOnly)
            {
                string text = dvalue.ToLongDateString();
                switch (this.clipboardFormat)
                {
                    case DateTimePickerFormat.Long: text = dvalue.ToLongDateString(); 
                        break;
                    case DateTimePickerFormat.Short: text = dvalue.ToShortDateString(); 
                        break;
                    case DateTimePickerFormat.Time: text = dvalue.ToLongTimeString();
                        break;
                    case DateTimePickerFormat.Custom:
                        {
                            text = string.Empty;
                            for (int i = 0; i < fields.Count; i++)
                            {
                                text += Fields(i).Text;
                            }
                            break;
                        }
                }
                Clipboard.SetDataObject(text);
                this.IsNullDate = true;
                return true;
            }
            else
            {
                if (selectedField != null)
                {
                    Clipboard.SetDataObject(selectedField.Text);
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Copies <see cref="Value"/> property to clipboard.
        /// </summary>
        /// <returns>
        /// True if value has been sucessfully copied.
        /// </returns>
        public bool Copy()
        {
            if (!this.copyFieldOnly)
            {
                string text = dvalue.ToLongDateString();
                switch (this.clipboardFormat)
                {
                    case DateTimePickerFormat.Long: text = dvalue.ToLongDateString();
                        break;
                    case DateTimePickerFormat.Short: text = dvalue.ToShortDateString();
                        break;
                    case DateTimePickerFormat.Time: text = dvalue.ToLongTimeString(); 
                        break;
                    case DateTimePickerFormat.Custom:
                        {
                            text = string.Empty;
                            for (int i = 0; i < fields.Count; i++)
                            {
                                text += Fields(i).Text;
                            }
                            break;
                        }
                }
                Clipboard.SetDataObject(text);
            }
            else
            {
                if (selectedField != null)
                {
                    Clipboard.SetDataObject(selectedField.Text);
                }
                else
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Pastes clipboard data to <see cref="Value"/> property.
        /// </summary>
        /// <returns>
        /// True if clipboard data has been sucessfully pasted.
        /// </returns>
        [Description("Pastes clipboard data to Value property.")]
        public bool Paste()
        {
            string text = string.Empty;
            if ((text = (string)Clipboard.GetDataObject().GetData(typeof(string))) != null)
            {
                try
                {
                    DateTime dt = DateTime.Parse(text);
                    this.Value = dt;
                    if (isNullDate)
                        IsNullDate = false;
                }
                catch
                { 
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        private void CutMI_Click(object sender, System.EventArgs e)
        {
            Cut();
        }

        private void CopyMI_Click(object sender, System.EventArgs e)
        {
            Copy();
        }

        private void PasteMI_Click(object sender, System.EventArgs e)
        {
            Paste();
        }

        protected override void OnBorderStyleChanged()
        {
            base.OnBorderStyleChanged();
        }

        private void NoDateTimeMI_Click(object sender, System.EventArgs e)
        {
            if (this.customPopup)
            {
                if (this.customPopupWindow != null)
                    this.customPopupWindow.HidePopup(PopupCloseType.Canceled);
            }
            else
                this.popupWindow.HidePopup(PopupCloseType.Canceled);

            IsNullDate = !isNullDate;
        }

        private void ContextMenu_Popup(object sender, System.EventArgs e)
        {
            this.contextMenu.CutEnabled = !isNullDate;
            this.contextMenu.CopyEnabled = !isNullDate;
            this.contextMenu.PasteEnabled = false;

            string text = Clipboard.GetDataObject().GetData(typeof(string)) as string;

            if (text != null)
            {
                try
                {
                    this.contextMenu.PasteEnabled = true;
                }
                catch 
                { 
                }
            }

            this.contextMenu.ShowNoDateTime = this.enableNullDate;
            this.contextMenu.NoDateTimeChecked = this.isNullDate;
        }

        private bool GetIsMirrored()
        {
            return RightToLeft.Yes == base.RightToLeft;
        }

        private void DateTimePickerAdv_MouseEnter(object sender, System.EventArgs e)
        {
            if (this.Style == VisualStyle.OfficeXP)
            {
                this.StyleBorderColor = MenuColors.SelBorderColor;
            }
            if (this.Style == VisualStyle.Office2003)
            {
                this.StyleBorderColor = Office2003Colors.SelBorderColor;
            }
            if (this.Style == VisualStyle.Metro)
            {
                this.StyleBorderColor = MetroColor;
            }
            this.dropButton.Selected = true;
            this.dropButton.Invalidate();

            this.numericUD.Selected = true;
            this.numericUD.Invalidate();

            this.checkBox.Selected = true;
            this.checkBox.Invalidate();
        }

        private void DateTimePickerAdv_MouseLeave(object sender, System.EventArgs e)
        {
            bool shouldChange = true;
            foreach (Control c in this.Controls)
            {
                if (this.RectangleToScreen(c.Bounds).Contains(MousePosition))
                    shouldChange = false;
            }

            if (!Focused && shouldChange)
            {
                if ((this.Style == VisualStyle.OfficeXP || this.Style == VisualStyle.Office2003 ||
                    this.Style == VisualStyle.VS2005 || this.Style == VisualStyle.Office2007 || this.Style == VisualStyle.Office2010)
                    && !this.dropButton.Dropped)
                {
                    if (this.Style == VisualStyle.OfficeXP || this.Style == VisualStyle.Office2003)
                        this.StyleBorderColor = MenuColors.DropDownBorderColor;

                    this.dropButton.Selected = false;
                    this.dropButton.Invalidate();

                    this.numericUD.Selected = false;
                    this.numericUD.Invalidate();

                    this.checkBox.Selected = false;
                    this.checkBox.Invalidate();
                }
            }
        }

        protected override void OnBorderColorChanged()
        {
            OnBorderColorChangedInternal();

            base.OnBorderColorChanged();
        }

        private void OnBorderColorChangedInternal()
        {
            this.dropButton.FlatColor = BorderColorInternal;
        }

        void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (this.UseCurrentCulture && e.Category == UserPreferenceCategory.Locale)
            {
                this.Culture = new System.Globalization.CultureInfo((int)NativeMethods.GetUserDefaultLCID(), false);
            }
        }

        /// <summary>
        /// Handles invalid selected date and restores it to default.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e"> EventArgs that contains the event data.</param>
        private void MonthCalendar_InvalidDateSelected(object sender, EventArgs e)
        {
            monthCalendar.Value = dateOnPopup;
        }

        private void Office2007ManagedColorsApplied(Office2007Colors.ManagedColorsAppliedEventArgs args)
        {
            if (this.Style == VisualStyle.Office2007)
            {
                ApplyOffice2007Theme();
            }
        }
        private void Office2010ManagedColorsApplied(Office2010Colors.ManagedColorsAppliedEventArgs args)
        {
            if (this.Style == VisualStyle.Office2010)
            {
                ApplyOffice2010Theme();
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
         internal static extern int AdjustWindowRectEx(ref NativeMethods.RECT lpRect, int dwStyle, bool bMenu, int dwExStyle);
    }
    [ToolboxItem(false)]
    public class DTField
    {
        private FieldDefinition definition;
        private int maxWidth = 0;
        private int maximum = 10000;
        private Point location;
        private string text;
        private DateTimePickerAdv picker;
        private int width;
        private int textWidth;
        private int height;
        private bool startedTyping;

        /// <summary>
        /// Position for insert next number.
        /// </summary>
        private int m_iInsertPosYear = 0;

        /// <summary>
        /// Indicates whether to skip check year validating.
        /// </summary>
        internal bool M_bSkipYearCheck = false;

        private bool rightAlign = false;
        public event EventHandler TextChanged;

        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(this, e);
            }
        }

        internal bool RightAlign
        {
            get 
            { 
                return rightAlign; 
            }
            set 
            { 
                rightAlign = value;
            }
        }

        internal bool StartedTyping
        {
            get 
            { 
                return startedTyping; 
            }
            set 
            { 
                startedTyping = value; 
            }
        }

        /// <summary>
        /// Gets or sets position for insert next number.
        /// </summary>
        internal int InsertPosYear
        {
            get
            {
                return m_iInsertPosYear;
            }
            set
            {
                if (m_iInsertPosYear != value)
                {
                    m_iInsertPosYear = value;
                }
            }
        }

        internal int Width
        { 
            get
            { 
                return width; 
            } 
            set 
            { 
                width = value; 
            }
        }
        internal int Height
        {
            get
            {
                return height;
            }
        
            set 
            {
                height = value;
            } 
        }

        internal Rectangle Bounds
        {
            get 
            { 
                return new Rectangle(location.X, location.Y, width, height); 
            }
            set 
            { 
                location = value.Location; 
                width = value.Width; 
                height = value.Height; 
            }
        }
        public string Text
        {
            get
            { 
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnTextChanged(EventArgs.Empty);
                }
                Text_Changed();
            }
        }
        internal Rectangle TextRectangle
        {
            get
            {
                return new Rectangle(MiddleLocation, new Size(textWidth, height));
            }
        }
        internal Point MiddleLocation
        {
            get
            {
                if (!this.rightAlign)
                {
                    return new Point(location.X + (width - textWidth) / 2, location.Y);
                }
                else
                {
                    return new Point(location.X + this.width - textWidth, location.Y);
                }
            }
        }

        internal Point Location
        {
            get { return location; }
            set { location = value; }
        }

        internal int Maximum
        {
            get 
            { 
                return maximum; 
            }
            set
            {
                maximum = value;
            }
        }

        internal int MaxWidth
        {
            get 
            { 
                return maxWidth; 
            }
            set 
            { 
                maxWidth = value;
            }
        }

        public FieldDefinition Definition
        {
            get 
            { 
                return definition; 
            }
            set 
            { 
                definition = value;
            }
        }

        public DTField(FieldDefinition fd, DateTimePickerAdv dtPicker)
        {
            definition = fd;
            this.picker = dtPicker;
        }

        private void Text_Changed()
        {
            if (text == string.Empty)
            {
                width = height = 0;
                return;
            }

            Graphics g = picker.CreateGraphics();
            Size size = g.MeasureString(Text, picker.Font).ToSize();
            int newWidth = size.Width;

            if (text.Trim() == text)
            {
                newWidth = MeasureString(g, text, picker.Font) - 2;
            }

            Width = textWidth = newWidth;

            Height = size.Height;

            g.Dispose();
        }

        /// <summary>
        /// Resets insert year mode.
        /// </summary>
        internal void ResetInsertYearMode()
        {
            this.InsertPosYear = 0;
            this.M_bSkipYearCheck = false;
        }

        internal bool ProcessNumberKeyYear(int val)
        {
            if (picker.ReadOnly) return false;

            int formatLength = (int)YearFormat.Full;
            string sValue = this.Text;

            this.InsertPosYear++;
            sValue += val.ToString();

            if (sValue.Length > formatLength)
            {
                sValue = sValue.Remove(0, sValue.Length - formatLength);
            }

            this.Text = sValue;

            bool needNextField = false;

            if (this.InsertPosYear >= formatLength)
            {
                this.ResetInsertYearMode();
                needNextField = true;
            }
            else
            {
                M_bSkipYearCheck = true;
            }

            return needNextField;
        }

        /// <summary>
        /// Parse current value with picker and field type.
        /// </summary>
        /// <returns> returns the current value of type int</returns>
        private int GetCurrentValue()
        {
            int returnValue = -1;

            if (this.picker != null)
            {
                if (this.Definition.Type == FieldType.Month)
                {
                    returnValue = this.picker.Value.Month;
                }
                else if (this.Definition.Type == FieldType.Day && this.Definition.DayFormat == DayFormat.FullName)
                {
                    returnValue = this.picker.Value.Day;
                }
            }

            return returnValue;
        }

        internal bool ProcessNumberKey(int val)
        {
            if (picker.ReadOnly) return false;

            int ivalue;
            bool first = false;
            if (!startedTyping)
            {
                startedTyping = true;
                Text = "0";
                first = true;
            }
            try
            {
                ivalue = int.Parse(Text);
            }
            catch
            {
                ivalue = GetCurrentValue();

                if (ivalue == -1)
                {
                    return false;
                }
            }
            bool zero = ivalue == 0;
            if (ivalue * 10 + val > maximum)
            {
                ivalue = val;
            }
            else
            {
                ivalue = ivalue * 10 + val;
            }
            Text = (!zero || (picker.AutoForwarding == true && first) ? string.Empty  : "0") + ivalue.ToString();
            if (ivalue * 10 > maximum)
            {
                return true;
            }
            if (picker.AutoForwarding)
            {
                return Text.Length == this.maximum.ToString().Length;
            }
            return ivalue.ToString().Length == this.maximum.ToString().Length;
        }

        private static StringFormat s_sfMeasure;

           internal static int MeasureString(Graphics graphics, string text, Font font)
           {
               if (s_sfMeasure == null)
               {
                   s_sfMeasure = new StringFormat();
               }
               lock (s_sfMeasure)
               {
                   RectangleF rect = new RectangleF(0, 0, 1000, 1000);
                   CharacterRange[] ranges = { new System.Drawing.CharacterRange(0, text.Length) };
                   Region[] regions = new Region[1];

                   s_sfMeasure.SetMeasurableCharacterRanges(ranges);

                   regions = graphics.MeasureCharacterRanges(text, font, rect, s_sfMeasure);
                   rect = regions[0].GetBounds(graphics);
                   return (int)rect.Right/* + 1.0f*/;
               }
          }
    }
    public enum NullModeKeyReset
    {
        /// <summary>
        /// Represents Arrow Keys
        /// </summary>
        ArrowKeys,

        /// <summary>
        /// Represents Numeric keys
        /// </summary>
        NumericKeys,

        /// <summary>
        /// Represents any key
        /// </summary>
        Any
    }

    /// <summary>
    /// The DateTimePickerExt type will soon be replaced with the DateTimePickerAdv for consistency in 
    /// Control naming in our library. 
    /// Please replace all occurrences of DateTimePickerExt with DateTimePickerAdv in your app.
    /// </summary>
    [Obsolete("The DateTimePickerExt type will soon be replaced with the DateTimePickerAdv for consistency in naming in our library. Please replace all occurences of DateTimePickerExt with DateTimePickerAdv in your app."),
    ToolboxItem(false)]
    public class DateTimePickerExt : DateTimePickerAdv
    {
    }

    public class DateTimePickerAdvDesigner : System.Windows.Forms.Design.ControlDesigner
    {
        public DateTimePickerAdvDesigner()
            : base()
        {
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )

       private System.ComponentModel.Design.DesignerActionListCollection actionLists;

        public override System.ComponentModel.Design.DesignerActionListCollection ActionLists
        {
            get
            {
                if (null == actionLists)
                {
                    actionLists = new System.ComponentModel.Design.DesignerActionListCollection();
                    actionLists.Add(
                        new DateTimePickerAdvActionList(this.Component));
                }
                return actionLists;
            }
        }
#endif
    }

    internal class DateTimeEditorAdv :
        UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc != null)
            {
                MonthCalendarAdv mc = new MonthCalendarAdv();

                ((ISupportInitialize)mc).BeginInit();

                mc.DaysFont = new Font("Verdana", 8F);
                mc.HeaderFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)0));
                mc.Style = VisualStyle.Default;
                mc.WeekFont = new Font("Verdana", 8F);
                mc.WeekInterior = new BrushInfo(GradientStyle.Vertical, Color.PeachPuff, Color.AntiqueWhite);

                mc.NoneButton.Visible = false;
                mc.TodayButton.Appearance = ButtonAppearance.WindowsXP;
                mc.TodayButton.BackColor = SystemColors.Control;

                ISupportCulture sc = context.Instance as ISupportCulture;

                if (sc != null)
                {
                    mc.Culture = sc.Culture;
                }

                mc.DateSelected += new EventHandler(OnDateSelected);

                ((ISupportInitialize)mc).EndInit();

                mc.Value = (DateTime)value;

                edSvc.DropDownControl(mc);

                value = mc.Value;

                mc.DateSelected -= new EventHandler(OnDateSelected);
            }
            return value;
        }

       public void OnDateSelected(object sender, EventArgs e)
        {
            edSvc.CloseDropDown();
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
