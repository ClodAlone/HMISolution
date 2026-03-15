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
using System.ComponentModel;

namespace Syncfusion.Windows.Diagram
{
    public class DateTimeSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Convert DateTime to double value.
        /// </summary>
        public double ToPixel(DateTime dt)
        {
            return dt.ToPixel(this);
        }

        /// <summary>
        /// Convert TimeSpan to double value.
        /// </summary>
        public double ToPixel(TimeSpan ts)
        {
            return ts.ToPixel(this);
        }

        /// <summary>
        /// Convert double to DateTime value.
        /// </summary>
        public DateTime ToDateTime(double d)
        {
            return d.ToDateTime(this);
        }

        /// <summary>
        /// Convert double to TimeSpan value.
        /// </summary>
        public TimeSpan ToTimeSpan(double d)
        {
            return d.ToTimeSpan(this);
        }

        private TimeSpan m_RulerInterval = new TimeSpan(1, 0, 0, 0);

        /// <summary>
        /// Used to define distance between Majorlines in Ruler.
        /// </summary>
        public TimeSpan RulerInterval
        {
            get { return m_RulerInterval; }
            set { m_RulerInterval = value; OnPropertyChanged("RulerInterval"); }
        }

        private string m_CustomFormatString = "{0:d}";

        /// <summary>
        /// Format of the DateTime value that is displayed in Ruler.
        /// </summary>
        public string CustomFormatString
        {
            get { return m_CustomFormatString; }
            set { m_CustomFormatString = value; OnPropertyChanged("CustomFormatString"); }
        }

        private DateTimeFormatSetting m_DateTimeFormatSetting = DateTimeFormatSetting.Custom;

        /// <summary>
        /// Future Enhancement - Dynamic FormatString based on 
        /// </summary>
        internal DateTimeFormatSetting FormatString
        {
            get { return m_DateTimeFormatSetting; }
            set { m_DateTimeFormatSetting = value; /*OnPropertyChanged("DateTimeFormatString");*/ }
        }

        private bool m_IsEnabled = false;

        /// <summary>
        /// Gets or sets a value indicating whether DateTime feature is enabled or not.
        /// </summary>
        /// <remarks>Default value: false. If this property is set to false, Node's DateTime related properties will have no effect and the Rulers will not show DateTime value.</remarks>
        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set { m_IsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        private TimeSpan m_TimeSpan = new TimeSpan(1, 0, 0, 0);

        /// <summary>
        /// Gets the TimeSpan for an unit.
        /// </summary>
        /// <value></value>
        /// <remarks>Eg: If 100 pixels is considered as 1 day then TimeSpan = 1 day and PixelUnit = 100.</remarks>
        public TimeSpan TimeSpan
        {
            get { return m_TimeSpan; }
            set { m_TimeSpan = value; OnPropertyChanged("TimeSpan"); }
        }

        private double m_PixelUnit = 100d;

        /// <summary>
        /// Gets the number of pixel to be considered for TimeSpan specified.
        /// </summary>
        /// <value></value>
        /// <remarks>Eg: If 100 pixels is considered as 1 day then TimeSpan = 1 day and PixelUnit = 100.</remarks>
        public double PixelUnit
        {
            get { return m_PixelUnit; }
            set { m_PixelUnit = value; OnPropertyChanged("PixelUnit"); }
        }

        private DateTime m_OriginDateX = new DateTime(2000, 1, 1);

        /// <summary>
        /// Gets or sets Origin DateTime that represents origin if DiagramPage.
        /// </summary>
        /// <value>Default value: 1/1/2000</value>
        public DateTime OriginDateX
        {
            get { return m_OriginDateX; }
            set { m_OriginDateX = value; OnPropertyChanged("OriginDateX"); }
        }

        private DateTime m_OriginDateY = new DateTime(2000, 1, 1);

        /// <summary>
        /// Gets or sets Origin DateTime that represents origin if DiagramPage.
        /// </summary>
        /// <value>Default value: 1/1/2000</value>
        public DateTime OriginDateY
        {
            get { return m_OriginDateY; }
            set { m_OriginDateY = value; OnPropertyChanged("OriginDateY"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Diagram.DateTimeFactor">DateTimeFactor</see> class. 
        /// </summary>
        /// <param name="timeSpan">TimeSpan for an unit</param>
        /// <param name="pixelPerUnit">number of pixel to be considered for TimeSpan specified</param>
        /// <remarks>Eg: If 100 pixels is considered as 1 day then TimeSpan = 1 day and PixelUnit = 100.</remarks>
        public DateTimeSettings(TimeSpan timeSpan, double pixelPerUnit)
        {
            TimeSpan = timeSpan;
            PixelUnit = pixelPerUnit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.Windows.Diagram.DateTimeSettings">DateTimeSettings</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public DateTimeSettings()
        {
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Occurs when Properties of DateTimeSettings are changed. 
        /// </summary>
        /// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
