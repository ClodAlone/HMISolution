#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
namespace Syncfusion.Windows.Controls.Schedule
{
	/// <summary>
	/// Provides a converter to convert <see cref="DateTime"/> objects to and from <see cref="String"/> representations.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class DateTimeTypeConverter :
		TypeConverter
	{
		/// <summary>
		/// Returns whether the type converter can convert an object from the specified type to the type of this converter.
		/// </summary>
		/// <param name="context">An object that provides a format context.</param>
		/// <param name="sourceType">The type you want to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string);
		}

		/// <summary>
		/// Returns whether the type converter can convert an object to the specified type.
		/// </summary>
		/// <param name="context">An object that provides a format context.</param>
		/// <param name="destinationType">The type you want to convert to.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		/// <summary>
		/// Converts from the specified value to the type of this converter.
		/// </summary>
		/// <param name="context">An object that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The value to convert to the type of this converter.</param>
		/// <returns>The converted value.</returns>
		/// <exception cref="T:System.NotImplementedException">
		/// <see cref="M:System.ComponentModel.TypeConverter.ConvertFrom(System.ComponentModel.ITypeDescriptorContext,System.Globalization.CultureInfo,System.Object)"/> not implemented in base <see cref="T:System.ComponentModel.TypeConverter"/>.
		/// </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}

			if (value == null)
			{
				throw new ArgumentNullException("value");
			}

			Type sourceType = value.GetType();

			if (!this.CanConvertFrom(sourceType))
			{
				throw new ArgumentException(string.Format("Can't convert value from type '{0}'.", sourceType.FullName));
			}

			return DateTime.Parse(value.ToString(), culture.DateTimeFormat);
		}

		/// <summary>
		/// Converts the specified value object to the specified type.
		/// </summary>
		/// <param name="context">An object that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The object to convert.</param>
		/// <param name="destinationType">The type to convert the object to.</param>
		/// <returns>The converted object.</returns>
		/// <exception cref="T:System.NotImplementedException">
		/// <see cref="M:System.ComponentModel.TypeConverter.ConvertTo(System.ComponentModel.ITypeDescriptorContext,System.Globalization.CultureInfo,System.Object,System.Type)"/>  not implemented in base <see cref="T:System.ComponentModel.TypeConverter"/>.
		/// </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}

			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}

			if (!this.CanConvertTo(destinationType))
			{
				throw new ArgumentException(string.Format("Can't convert value to type '{0}'.", destinationType.FullName));
			}

			DateTime dt = (DateTime)value;

			return dt.ToString(culture.DateTimeFormat);
		}
	}
    /// <summary>
    /// Provides a converter to convert <see cref="bool"/> objects to and from <see cref="bool"/> representations.
    /// </summary>
    public class ScheduleAllDayCheckedToEnabledConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var allDayResult = (bool)value;
            if (allDayResult)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
    /// <summary>
    /// Provides a converter to convert <see cref="bool"/> objects to and from <see cref="bool"/> representations.
    /// </summary>
    public class ScheduleRecurrenceIsCheckedConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rdoResult = (bool)value;
            return !rdoResult;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rdoResult = (bool)value;
            return !rdoResult;
        }

        #endregion
    }


    //public class StringAppointmentPriorityConverter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        string val = value as string;
    //        //if (targetType == typeof(AppointmentPriority))
    //        //{
    //        //    switch (val)
    //        //    {
    //        //        case "Free":
    //        //            result = (object)AppointmentPriority.Free;
    //        //            break;
    //        //        case "Busy":
    //        //            result = (object)AppointmentPriority.Busy;
    //        //            break; 
    //        //        case "Tentative":
    //        //            result = (object)AppointmentPriority.Tentative;
    //        //            break;
    //        //        case "Out-Of-Office":
    //        //            result = (object)AppointmentPriority.OutOfOffice;
    //        //            break;
    //        //        default :
    //        //            result = (object)AppointmentPriority.Free;
    //        //            break;
    //        //    }

    //        //}

    //        return result;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        //AppointmentPriority? val = value as AppointmentPriority?;         
    //        //if (targetType == typeof(string))
    //        //{
    //        //    switch (val)
    //        //    {
    //        //        case AppointmentPriority.Free:
    //        //            result = (object)"Free";
    //        //            break;
    //        //        case AppointmentPriority.Busy:
    //        //            result = (object)"Busy";
    //        //            break;
    //        //        case AppointmentPriority.Tentative:
    //        //            result = (object)"Tentative";
    //        //            break;
    //        //        case AppointmentPriority.OutOfOffice:
    //        //            result = (object)"Out-Of-Office";
    //        //            break;
    //        //        default:
    //        //            result = (object)"Free";
    //        //            break;
    //        //    }

    //        //}
    //        return result;
    //    }

    //    #endregion
    //}

	/// <summary>
	/// Abstract <see cref="ScheduleAppointment"/> to string converter.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

	public abstract class AppointmentToStringConverter :
		IValueConverter
	{
		#region IValueConverter implementation

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object result = null;
			ScheduleAppointment app = value as ScheduleAppointment;

			if (app != null)
			{
				if (typeof(string) == targetType || typeof(object) == targetType)
				{
					result = this.Convert(app);
				}
				else
				{
					throw new ArgumentException(String.Format("Can't convert to {0}. Only conversion to string is supported.", targetType.FullName));
				}
			}

			return result;
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Converts the specified <see cref="ScheduleAppointment"/> to string.
		/// </summary>
		/// <param name="app">The appointment to be converted.</param>
		/// <returns>String representation of <see cref="ScheduleAppointment"/>.</returns>
		public abstract string Convert(ScheduleAppointment app);

		#endregion
	}

    #region Comment - TODO CHECK LATER

    ///// <summary>
    ///// Converts <see cref="ScheduleAppointment.Tooltip"/> property to appropriate string tooltip content.
    ///// </summary>
    //public class AppointmentTooltipContentConverter :
    //    AppointmentToStringConverter
    //{
    //    /// <summary>
    //    /// Converts the specified <see cref="ScheduleAppointment"/> to string.
    //    /// </summary>
    //    /// <param name="app">The appointment to be converted.</param>
    //    /// <returns>
    //    /// String representation of <see cref="ScheduleAppointment"/>.
    //    /// </returns>
    //    public override string Convert(ScheduleAppointment app)
    //    {
    //        return RetrieveTooltipContent(app);
    //    }

    //    internal static string RetrieveTooltipContent(ScheduleAppointment app)
    //    {
    //        string toolTipContent = null;
    //        CultureInfo currentCulture = CultureInfo.CurrentCulture;

    //        switch (app.Tooltip)
    //        {
    //            case TooltipType.Location:
    //                {
    //                    toolTipContent = app.Location;

    //                    break;
    //                }



    //            case TooltipType.StartTime:
    //                {
    //                    toolTipContent = app.StartTime.ToLongTimeString();

    //                    break;
    //                }

    //            case TooltipType.EndTime:
    //                {
    //                    toolTipContent = app.EndTime.ToLongTimeString();

    //                    break;
    //                }

    //            case TooltipType.StartAndEndTime:
    //                {
    //                    toolTipContent = String.Format(currentCulture, GetDateTimeStringFormat(app), app.StartTime, app.EndTime);

    //                    break;
    //                }

    //            case TooltipType.All:
    //                {
    //                    StringBuilder sb = new StringBuilder();
    //                    string line = app.Location;

    //                    if (!String.IsNullOrEmpty(line))
    //                    {
    //                        sb.AppendLine(line);
    //                    }

    //                    if (!app.AllDay)
    //                    {

    //                        if (!String.IsNullOrEmpty(line))
    //                        {
    //                            sb.AppendLine(line);
    //                        }
    //                    }

    //                    line = String.Format(currentCulture, GetDateTimeStringFormat(app), app.StartTime, app.EndTime);
    //                    sb.AppendFormat(line);

    //                    line = app.Subject;

    //                    if (!String.IsNullOrEmpty(line))
    //                    {
    //                        sb.AppendLine();
    //                        sb.Append(line);
    //                    }

    //                    toolTipContent = sb.ToString();

    //                    break;
    //                }

    //            case TooltipType.Custom:
    //                {
    //                    toolTipContent = app.CustomTooltip;

    //                    break;
    //                }
    //        }

    //        return toolTipContent;
    //    }

    //    private static string GetDateTimeStringFormat(ScheduleAppointment app)
    //    {
    //        return String.Format("{{0:{0}}} - {{1:{0}}}", app.AllDay ? "d" : "hh:mm");
    //    }
    //}
    
    #endregion

	/// <summary>
	/// Converts <see cref="ScheduleAppointment.StartTime"/> and <see cref="ScheduleAppointment.EndTime"/> range to its string short time representation.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class AppointmentDateRangeConverter :
		AppointmentToStringConverter
	{
		/// <summary>
		/// Converts the specified <see cref="ScheduleAppointment"/> to string.
		/// </summary>
		/// <param name="app">The appointment to be converted.</param>
		/// <returns>
		/// String representation of <see cref="ScheduleAppointment"/>.
		/// </returns>
		public override string Convert(ScheduleAppointment app)
		{
            return String.Format(CultureInfo.CurrentCulture, "{0:hh:mm}-{1:hh:mm}", app.StartTime, app.EndTime);
		}
	}


    //public class StringToAppointmentPriorityConverter : IValueConverter
    //{


    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        //if (value != null && value.GetType() == typeof(AppointmentPriority) && targetType.GetType() == typeof(string))
    //        //{
    //        //    AppointmentPriority val = (AppointmentPriority)value;
    //        //    result = value.ToString();
    //        //}
    //        return result;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {         
    //        object result = null;

    //        //if (value != null && value.GetType() == typeof(string) && targetType == typeof(AppointmentPriority))
    //        //{
    //        //    string val = (string)value;
    //        //    if (val == "Free") result = AppointmentPriority.Free;
    //        //    else if (val == "Busy") result = AppointmentPriority.Busy;
    //        //    else if (val == "Tentative") result = AppointmentPriority.Tentative;
    //        //    else if (val == "Out-Of-Office") result = AppointmentPriority.OutOfOffice;
    //        //}
    //        return result;
    //    }

    //    #endregion
    //}

    //public class PriorityConverter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        //if (value != null && value.GetType() == typeof(AppointmentPriority))
    //        //{
    //        //    AppointmentPriority val = (AppointmentPriority)value;
    //        //    if (val == AppointmentPriority.Free)
    //        //        result = 0;
    //        //    else if (val == AppointmentPriority.Tentative)
    //        //        result = 1;
    //        //    else if (val == AppointmentPriority.Busy)
    //        //        result = 2;
    //        //    else if (val == AppointmentPriority.OutOfOffice)
    //        //        result = 3;
    //        //}

    //        return result;            
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        //if (value != null && value.GetType() == typeof(int))
    //        //{
    //        //    int val = (int)value;
    //        //    if (val == 0)
    //        //        result = AppointmentPriority.Free;
    //        //    else if (val == 1)
    //        //        result = AppointmentPriority.Tentative;
    //        //    else if (val == 2)
    //        //        result = AppointmentPriority.Busy;
    //        //    else if (val == 3)
    //        //        result = AppointmentPriority.OutOfOffice;
    //        //}
    //        return result;
    //    }

    //    #endregion
    //}

	/// <summary>
	/// Converts <see cref="bool"/> value to <see cref="Visibility"/> value and vice-versa.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class BoolToVisibilityConverter :
		IValueConverter
	{
		#region IValueConverter implementation

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object result = null;

			if (value != null && value.GetType() == typeof(bool) && targetType == typeof(Visibility))
			{
				bool vis = (bool)value;

				if (parameter != null && (string)parameter == "Not")
				{
					vis = !vis;
				}

				result = vis ? Visibility.Visible : Visibility.Collapsed;				
			}
            
			return result;
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object result = null;

			if (value != null && value.GetType() == typeof(Visibility) && targetType == typeof(bool))
			{
				Visibility vis = (Visibility)value;

				result = vis == Visibility.Visible;
			}			

			return result;			
		}

		#endregion
	}

	/// <summary>
	/// Converts <see cref="DateTime"/> value to formatted string for week day.
	/// </summary>
    /// <remarks>Format must be passed as converter parameter.</remarks>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    public class WeekDayDateTimeFormatConverter :
		IValueConverter
	{
		#region IValueConverter Members

		/// <summary>
		/// Modifies the source data before passing it to the target for display in the UI.
		/// </summary>
		/// <param name="value">The source data being passed to the target.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the target dependency property.
		/// </returns>
		object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			object result = null;

			if (value is DateTime && typeof(string) == targetType)
			{
				DateTime dt = (DateTime)value;
				string param = parameter as string;

				if (string.IsNullOrEmpty(param))
				{
					result = dt.ToString(culture);
				}
				else
				{
					string[] ps = param.Split('|');

					if (ps.Length > 1)
					{
						param = ps[dt.Day != 1 ? 0 : 1];
					}
					
					result = String.Format(culture, param, dt);
				}
			}

			return result;
		}

		/// <summary>
		/// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
		/// </summary>
		/// <param name="value">The target data being passed to the source.</param>
		/// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
		/// <param name="parameter">An optional parameter to be used in the converter logic.</param>
		/// <param name="culture">The culture of the conversion.</param>
		/// <returns>
		/// The value to be passed to the source object.
		/// </returns>
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}


    /// <summary>
    /// Provides a converter to convert <see cref="DateTime"/> objects to and from <see cref="String"/> representations.
    /// </summary>
    public class ThresholdTimeConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = null;
            TimeSpan ts = (TimeSpan)value;
            string choice = ts.ToString();

            switch (choice)
            {
                case "00:00:00":
                    result = "0 minutes";
                    break;
                case "00:05:00":
                    result = "5 minutes";
                    break;
                case "00:10:00":
                    result = "10 minutes";
                    break;
                case "00:15:00":
                    result = "15 minutes";
                    break;
                case "00:30:00":
                    result = "30 minutes";
                    break;
                case "01:00:00":
                    result = "1 hour";
                    break;
                case "02:00:00":
                    result = "2 hours";
                    break;
                case "03:00:00":
                    result = "3 hours";
                    break;
                case "04:00:00":
                    result = "4 hours";
                    break;
                case "05:00:00":
                    result = "5 hours";
                    break;
                case "06:00:00":
                    result = "6 hours";
                    break;
                case "07:00:00":
                    result = "7 hours";
                    break;
                case "08:00:00":
                    result = "8 hours";
                    break;
                case "09:00:00":
                    result = "9 hours";
                    break;
                case "10:00:00":
                    result = "10 hours";
                    break;
                case "11:00:00":
                    result = "11 hours";
                    break;
                case "12:00:00":
                    result = "0.5 day";
                    break;
                case "18:00:00":
                    result = "18 hours";
                    break;
                case "01:00:00:00":
                    result = "1 day";
                    break;
                case "02:00:00:00":
                    result = "2 days";
                    break;
                case "03:00:00:00":
                    result = "3 days";
                    break;
                case "04:00:00:00":
                    result = "4 days";
                    break;
                case "07:00:00:00":
                    result = "1 week";
                    break;
                case "14:00:00:00":
                    result = "2 weeks";
                    break;
                default:
                    break;
            }
            return result;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan result=new TimeSpan(00,00,00);
            string val = (string)value;

            switch (val)
            {
                case "0 minutes":
                    result = new TimeSpan(00, 00, 00);
                    break;
                case "5 minutes":
                    result = new TimeSpan(00, 05, 00);
                    break;
                case "10 minutes":
                    result = new TimeSpan(00, 10, 00);
                    break;
                case "15 minutes":
                    result = new TimeSpan(00, 15, 00);
                    break;
                case "30 minutes":
                    result = new TimeSpan(00, 30, 00);
                    break;
                case "1 hour":
                    result = new TimeSpan(01, 00, 00);
                    break;
                case "2 hours":
                    result = new TimeSpan(02, 00, 00);
                    break;
                case "3 hours":
                    result = new TimeSpan(03, 00, 00);
                    break;
                case "4 hours":
                    result = new TimeSpan(04, 00, 00);
                    break;
                case "5 hours":
                    result = new TimeSpan(05, 00, 00);
                    break;
                case "6 hours":
                    result = new TimeSpan(06, 00, 00);
                    break;
                case "7 hours":
                    result = new TimeSpan(07, 00, 00);
                    break;
                case "8 hours":
                    result = new TimeSpan(08, 00, 00);
                    break;
                case "9 hours":
                    result = new TimeSpan(09, 00, 00);
                    break;
                case "10 hours":
                    result = new TimeSpan(10, 00, 00);
                    break;
                case "11 hours":
                    result = new TimeSpan(11, 00, 00);
                    break;
                case "0.5 day":
                    result = new TimeSpan(12, 00, 00);
                    break;
                case "18 hours":
                    result = new TimeSpan(18, 00, 00);
                    break;
                case "1 day":
                    result = new TimeSpan(01, 00, 00, 00);
                    break;
                case "2 days":
                    result = new TimeSpan(02, 00, 00, 00);
                    break;
                case "3 days":
                    result = new TimeSpan(03, 00, 00, 00);
                    break;
                case "4 days":
                    result = new TimeSpan(04, 00, 00, 00);
                    break;
                case "1 week":
                    result = new TimeSpan(07, 00, 00, 00);
                    break;
                case "2 weeks":
                    result = new TimeSpan(14, 00, 00, 00);
                    break;
            }
            return result;
        }

        #endregion
    }


//    public class PriorityBackgroundConverter : IValueConverter
//    {

//        #region IValueConverter Members

//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            object result = null;
//            //var val = (AppointmentPriority)value;
//            //Border b = new Border();
//            //b.Background = new SolidColorBrush(Colors.White);
//            //if (val == AppointmentPriority.Free)
//            //{
//            //    b.Background = new SolidColorBrush(Colors.White);
//            //    result = b;
//            //}
//            //else if (val == AppointmentPriority.OutOfOffice)
//            //{
//            //    b.Background = new SolidColorBrush(Colors.Purple);
//            //    result = b;
//            //}
//            //else if (val == AppointmentPriority.Busy)
//            //{
//            //    b.Background = new SolidColorBrush(Colors.Transparent);
//            //    result = b;
//            //}
//            //else if (val == AppointmentPriority.Tentative)
//            //{                
////                string test = "<Border Background='White' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'> <Path Fill='#FFB0B6BE'  Stretch='Fill' Height='19.917' HorizontalAlignment='Left' VerticalAlignment='Top' Width='5' Data='M0,6.250226 L4.9580002,0.8749997 L4.9580002,3.3334365 L0,8.7919998 M4.3213367E-07,13.124765 L4.9580007,7.0419998 L4.9580007,9.9165554 L4.3213367E-07,15.707999 M4.3213367E-07,2.25 L2.1250005,0 L4.3213367E-07,0 M2.1670003,19.87533 L5,16.125 L4.9583335,19.917'/> </Border>";
////#if !SILVERLIGHT                
////                byte[] byteArray = Encoding.ASCII.GetBytes( test );
////                MemoryStream stream = new MemoryStream( byteArray ); 
////                result = XamlReader.Load(stream);                
////#else
////                result = XamlReader.Load(test);
////#endif
//                //b.Child = new ScheduleCrosslineControl();
//                //result = b;
//            //
//            //}

//            return result;
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }

    //public class PriorityBackgroundConverterDaysView : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        object result = null;
    //        //var val=(AppointmentPriority)value;
    //        //Border b = new Border();
    //        //b.Background = new SolidColorBrush(Colors.White);
    //        //if (val == AppointmentPriority.Free)
    //        //{
    //        //    b.Background = new SolidColorBrush(Colors.White);
    //        //    result = b;
    //        //}
    //        //else if (val == AppointmentPriority.OutOfOffice)
    //        //{
    //        //    b.Background = new SolidColorBrush(Colors.Purple);
    //        //    result = b;
    //        //}
    //        //else if (val == AppointmentPriority.Busy)
    //        //{
    //        //    b.Background = new SolidColorBrush(Colors.Transparent);
    //        //    result = b;
    //        //}
    //        //else if (val == AppointmentPriority.Tentative)
    //        //{               
    //        //    b.Child = new ScheduleCrosslineControl();
    //        //    result = b;

    //        //}           
            
    //        return result;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }


    /// <summary>
    /// Provides a converter to convert <see cref="DateTime"/> objects to and from integer index.
    /// </summary>
    public class SelectedIndexToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.SelectedIndexToVisibilityConverter"/>
        /// class.
        /// </summary>
        public SelectedIndexToVisibilityConverter()
        {
        }

        #region IValueConverter
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((ScheduleType)value)
            {
                case ScheduleType.Day:
                    return 0;
                case ScheduleType.Month:
                    return 1;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion IValueConverter
    }
    /// <summary>
    /// Provides a converter to convert <see cref="DateTime"/> objects to and from <see cref="String"/> representations.
    /// </summary>
    public class ScheduleTextToSelectedDatesConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var selectedDates = value as ObservableCollection<DateTime>;
            if (selectedDates == null)
            {
                return value;
            }
            var firstDate = selectedDates[0];
            var lastDate = selectedDates[selectedDates.Count - 1];
            List<string> resultItems = new List<string>();
            var result = string.Empty;
            var calendar = CultureInfo.CurrentCulture.Calendar;
            if (selectedDates.Count == 1)
            {
                //result = selectedDates[0].ToLongDateString();
                //int commaPlace = result.IndexOf(',');
                //result = result.Substring(commaPlace + 2);

                result = selectedDates[0].ToString("M").TrimEnd(';') + ", " + selectedDates[0].Year;
            }
            else if (selectedDates.Count > 1 && selectedDates.Count <= 7)
            {
                if (firstDate.Month == lastDate.Month && firstDate.Year == lastDate.Year)
                {
                    string lastDateString;
                    if (lastDate.Day < 10)
                        lastDateString = "0" + lastDate.Day.ToString();
                    else
                        lastDateString = lastDate.Day.ToString();

                    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDateString + ", " + lastDate.Year.ToString();
                }
                //if (firstDate.Day < 10)
                //    //result = "0" + firstDate.Day.ToString() + " - " + lastDate.ToLongDateString();
                //    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDate.Day.ToString() + ", " + lastDate.Year.ToString();
                //else
                //    //result = firstDate.Day.ToString() + " - " + lastDate.ToLongDateString();
                //    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDate.Day.ToString() + ", " + lastDate.Year.ToString();
                else if (firstDate.Year == lastDate.Year)
                    //result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDate.ToLongDateString();
                    result = firstDate.ToString("M").TrimEnd(';') + " - " + lastDate.ToString("M").TrimEnd(';') + ", " + lastDate.Year;
                else
                    //result = firstDate.ToLongDateString() + " - " + lastDate.ToLongDateString();
                    result = firstDate.ToString("M").TrimEnd(';') + ", " + firstDate.Year + " - " + lastDate.ToString("M").TrimEnd(';') + ", " + lastDate.Year;
            }
            else
            {
                if (firstDate.Month == lastDate.Month && firstDate.Year == lastDate.Year)
                    result = firstDate.ToString("MMMM") + " " + firstDate.Year;
                    //result = firstDate.ToString("Y").TrimEnd(';');
                else if (firstDate.Year == lastDate.Year)
                    //result = firstDate.ToString("MMMM") + " - " + lastDate.ToString("Y").TrimEnd(';');
                    result = firstDate.ToString("MMMM") + " - " + lastDate.ToString("MMMM") + " " + lastDate.Year;
                else
                    //result = firstDate.ToString("Y").TrimEnd(';') + " - " + lastDate.ToString("Y").TrimEnd(';');
                    result = firstDate.ToString("MMMM") + " " + firstDate.Year + " - " + lastDate.ToString("MMMM") + " " + lastDate.Year;
            }
            return result;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
    /// <summary>
    /// Provides a converter to convert <see cref="DateTime"/> objects to and from <see cref="ScheduleAppointment"/> representations.
    /// </summary>
    public class RecurrenceStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ScheduleAppointment recApp = new ScheduleAppointment();
            string recurrenceString = value as string;
            if (recurrenceString != null && recurrenceString != "" && recurrenceString != string.Empty)
            {

                if (recurrenceString.Contains("RRULE:FREQ=DAILY") || recurrenceString.Contains("RRULE:FREQ=WEEKLY") || recurrenceString.Contains("RRULE:FREQ=MONTHLY") || recurrenceString.Contains("RRULE:FREQ=YEARLY"))
                    recApp.IsRecurrenceAppointment = true;
                else
                    return recApp;
                recurrenceString = recurrenceString.Substring(recurrenceString.IndexOf(':') + 1);
                string[] recurrenceValues = recurrenceString.Split(';');
                foreach (string val in recurrenceValues)
                {
                    string[] attributes = val.Split('=');
                    if (attributes[0] == "FREQ")
                    {
                        if (attributes[1] == "DAILY")
                        {
                            recApp.CurrentRecurrencePatternMode = RecurrencePatternMode.Daily;
                            recApp.IsDailySelected = true;
                        }
                        else if (attributes[1] == "WEEKLY")
                        {
                            recApp.CurrentRecurrencePatternMode = RecurrencePatternMode.Weekly;
                            recApp.IsWeeklySelected = true;
                        }
                        else if (attributes[1] == "MONTHLY")
                        {
                            recApp.CurrentRecurrencePatternMode = RecurrencePatternMode.Monthly;
                            recApp.IsMonthlySelected = true;
                        }
                        else if (attributes[1] == "YEARLY")
                        {
                            recApp.CurrentRecurrencePatternMode = RecurrencePatternMode.Yearly;
                            recApp.IsYearlySelected = true;
                        }
                    }
                    if (attributes[0] == "COUNT")
                    {
                        int countValue = 0;
                        if (Int32.TryParse(attributes[1], out countValue))
                        {
                            recApp.EndOccurenceCount = countValue;
                            recApp.IsEndAfter = true;
                        }
                    }
                    if (attributes[0] == "INTERVAL")
                    {
                        int intervalValue = 0;
                        if (Int32.TryParse(attributes[1], out intervalValue))
                        {
                            if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Daily)
                                recApp.DailyDays = intervalValue;
                            else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
                                recApp.WeeklyWeeks = intervalValue;
                            else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                            {
                                recApp.MonthlyMonth = intervalValue;
                                recApp.MonthlyMonthMulti = intervalValue;
                            }
                            else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                                recApp.YearlyYear = intervalValue;
                        }
                    }
                    if (attributes[0] == "BYDAY")
                    {
                        string[] daysString;
                        daysString = attributes[1].Split(',');
                        if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
                        {
                            foreach (string day in daysString)
                            {
                                if (day == "SU")
                                    recApp.IsWeeklySundaySelected = true;
                                else if (day == "MO")
                                    recApp.IsWeeklyMondaySelected = true;
                                else if (day == "TU")
                                    recApp.IsWeeklyTuesdaySelected = true;
                                else if (day == "WE")
                                    recApp.IsWeeklyWednesdaySelected = true;
                                else if (day == "TH")
                                    recApp.IsWeeklyThursdaySelected = true;
                                else if (day == "FR")
                                    recApp.IsWeeklyFridaySelected = true;
                                else if (day == "SA")
                                    recApp.IsWeeklySaturdaySelected = true;
                            }
                        }
                        else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                        {
                            recApp.MonthlyDaySelected = this.GetDayName(attributes[1]);                            
                        }
                        else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                        {
                            recApp.YearlyMultiDaySelected = this.GetDayName(attributes[1]);                            
                        }
                    }
                    if (attributes[0] == "BYMONTHDAY")
                    {
                        int dayValue = 0;
                        if (Int32.TryParse(attributes[1], out dayValue))
                        {
                            if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                            {
                                recApp.MonthlyDays = dayValue;
                                recApp.IsMonthlyCustomDays = true;                                
                            }
                            else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                            {
                                recApp.YearlyDays = dayValue;
                                recApp.IsYearlyCustomDays = true;
                            }
                        }
                    }
                    if (attributes[0] == "BYSETPOS")
                    {
                        int posValue = 0;
                        if (Int32.TryParse(attributes[1], out posValue))
                        {
                            if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                            {
                                recApp.MonthlyWeekOrderSelected = this.PositionName(posValue);
                                recApp.IsMonthlyCustomDays = false;
                                recApp.IsMonthlyMultiDays = true;                               
                            }
                            else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                            {
                                recApp.YearlyMultiWeekOrderSelected = this.PositionName(posValue);
                                recApp.IsYearlyCustomDays = false;
                                recApp.IsYearlyMultiDays = true;                           
                            }
                        }
                    }
                    if (attributes[0] == "BYMONTH")
                    {
                        int monthValue = 0;
                        if (Int32.TryParse(attributes[1], out monthValue))
                        {
                            recApp.YearlyMonthSelected = this.GetMonthName(monthValue);
                            recApp.YearlyMultiMonthSelected = this.GetMonthName(monthValue);
                        }                         
                    }

                }
            }
            return recApp;
        }

        private string GetMonthName(Int32 arg)
        {
            if (arg == 1)
                return "January";
            else if (arg == 2)
                return "February";
            else if (arg == 3)
                return "March";
            else if (arg == 4)
                return "April";
            else if (arg == 5)
                return "May";
            else if (arg == 6)
                return "June";
            else if (arg == 7)
                return "July";
            else if (arg == 8)
                return "August";
            else if (arg == 9)
                return "September";
            else if (arg == 10)
                return "October";
            else if (arg == 11)
                return "November";
            else if (arg == 12)
                return "December";
            else
                return ""; 
        }

        private string PositionName(Int32 arg)
        {
            if (arg == 1)
                return "first";
            else if (arg == 2)
                return "second";
            else if (arg == 3)
                return "third";
            else if (arg == 4)
                return "fourth";
            else if (arg == -1)
                return "last";
            else
                return "";
        }

        private string GetDayName(string arg)
        {
            if (arg == "SU")
                return "Sunday";
            else if (arg == "MO")
                return "Monday";
            else if (arg == "TU")
                return "Tuesday";
            else if (arg == "WE")
                return "Wednesday";
            else if (arg == "TH")
                return "Thursday";
            else if (arg == "FR")
                return "Friday";
            else if (arg == "SA")
                return "Saturday";
            else if (arg == "SU,MO,TU,WE,TH,FR,SA")
                return "day";
            else if (arg == "MO,TU,WE,TH,FR")
                return "weekday";
            else if (arg == "SU,SA")
                return "weekend day";
            else
                return "";
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string recurrenceString = "";
            ScheduleAppointment recApp = value as ScheduleAppointment;
            if (recApp != null)
            {
                if (recApp.IsRecurrenceAppointment == true)
                {
                    recurrenceString = "RRULE:FREQ=";
                    if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Daily)
                    {
                        if (recApp.IsDailyWeekDays == false)
                        {
                            recurrenceString = recurrenceString + "DAILY;";
                            recurrenceString = recurrenceString + this.GetCount(recApp);
                            recurrenceString = recurrenceString + this.GetInterval(recApp);
                        }
                        else
                        {
                            recurrenceString = recurrenceString + "WEEKLY;" ;
                            recurrenceString = recurrenceString + this.GetCount(recApp);                            
                            recurrenceString = recurrenceString + "BYDAY=MO,TU,WE,TH,FR;";
                        }
                    }
                    else if(recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
                    {
                        recurrenceString = recurrenceString + "WEEKLY;";
                        recurrenceString = recurrenceString + this.GetCount(recApp);
                        recurrenceString = recurrenceString + this.GetInterval(recApp);
                        recurrenceString = recurrenceString + this.GetByDay(recApp);
                        if(recurrenceString.Contains("INTERVAL"))
                        {
                            recurrenceString = recurrenceString + this.GetWeekStart();                                                     
                        }
                    }
                    else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                    {
                        recurrenceString = recurrenceString + "MONTHLY;";
                        recurrenceString = recurrenceString + this.GetCount(recApp);
                        recurrenceString = recurrenceString + this.GetInterval(recApp);
                        if (!recApp.IsMonthlyMultiDays)
                        {
                            recurrenceString = recurrenceString + this.GetByMonthDay(recApp);
                        }
                        else
                        {
                            recurrenceString = recurrenceString + this.GetByDay(recApp);
                            recurrenceString = recurrenceString + this.GetBySetPos(recApp);
                        }
                    }
                    else if (recApp.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                    {
                        recurrenceString = recurrenceString + "YEARLY;";
                        recurrenceString = recurrenceString + this.GetCount(recApp);
                        recurrenceString = recurrenceString + this.GetInterval(recApp);
                        if (!recApp.IsYearlyMultiDays)
                        {
                            recurrenceString = recurrenceString + this.GetByMonthDay(recApp);
                            recurrenceString = recurrenceString + this.GetByMonth(recApp);
                        }
                        else
                        {
                            recurrenceString = recurrenceString + this.GetByDay(recApp);
                            recurrenceString = recurrenceString + this.GetByMonth(recApp);
                            recurrenceString = recurrenceString + this.GetBySetPos(recApp); 
                        }
                    }
                } 
            }

            if (recurrenceString != "")
                recurrenceString = recurrenceString.Substring(0, recurrenceString.Length - 1);
            return recurrenceString;
        }

        private string GetWeekStart()
        {
            string weekStart = "";
            DayOfWeek weekStartDay = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            if (weekStartDay == DayOfWeek.Saturday)
                weekStart = "SA";
            else if (weekStartDay == DayOfWeek.Sunday)
                weekStart = "SU";
            else if (weekStartDay == DayOfWeek.Monday)
                weekStart = "MO";
            else if (weekStartDay == DayOfWeek.Tuesday)
                weekStart = "TU";
            else if (weekStartDay == DayOfWeek.Wednesday)
                weekStart = "WE";
            else if (weekStartDay == DayOfWeek.Thursday)
                weekStart = "TH";
            else if (weekStartDay == DayOfWeek.Friday)
                weekStart = "FR";
            weekStart = "WKST=" + weekStart + ";";
            return weekStart;
        }
        
        private string GetByMonth(ScheduleAppointment app)
        {
            string byMonth = "";
            if (!app.IsYearlyMultiDays)
            {
                if (app.YearlyMonthSelected == "January")
                    byMonth = "1";
                if (app.YearlyMonthSelected == "February")
                    byMonth = "2";
                if (app.YearlyMonthSelected == "March")
                    byMonth = "3";
                if (app.YearlyMonthSelected == "April")
                    byMonth = "4";
                if (app.YearlyMonthSelected == "May")
                    byMonth = "5";
                if (app.YearlyMonthSelected == "June")
                    byMonth = "6";
                if (app.YearlyMonthSelected == "July")
                    byMonth = "7";
                if (app.YearlyMonthSelected == "August")
                    byMonth = "8";
                if (app.YearlyMonthSelected == "Septemper")
                    byMonth = "9";
                if (app.YearlyMonthSelected == "October")
                    byMonth = "10";
                if (app.YearlyMonthSelected == "November")
                    byMonth = "11";
                if (app.YearlyMonthSelected == "December")
                    byMonth = "12";
            }
            else
            {
                if (app.YearlyMultiMonthSelected == "January")
                    byMonth = "1";
                if (app.YearlyMultiMonthSelected == "February")
                    byMonth = "2";
                if (app.YearlyMultiMonthSelected == "March")
                    byMonth = "3";
                if (app.YearlyMultiMonthSelected == "April")
                    byMonth = "4";
                if (app.YearlyMultiMonthSelected == "May")
                    byMonth = "5";
                if (app.YearlyMultiMonthSelected == "June")
                    byMonth = "6";
                if (app.YearlyMultiMonthSelected == "July")
                    byMonth = "7";
                if (app.YearlyMultiMonthSelected == "August")
                    byMonth = "8";
                if (app.YearlyMultiMonthSelected == "Septemper")
                    byMonth = "9";
                if (app.YearlyMultiMonthSelected == "October")
                    byMonth = "10";
                if (app.YearlyMultiMonthSelected == "November")
                    byMonth = "11";
                if (app.YearlyMultiMonthSelected == "December")
                    byMonth = "12";
 
            }
            if (byMonth != "")
            {
                byMonth = "BYMONTH=" + byMonth + ";";
            }
            return byMonth; 
        }

        private int GetByMonthValue(string monthName)
        {
            if (monthName == "January")
                return 1;
            if (monthName == "February")
                return 2;
            if (monthName == "March")
                return 3;
            if (monthName == "April")
                return 4;
            if (monthName == "May")
                return 5;
            if (monthName == "June")
                return 6;
            if (monthName == "July")
                return 7;
            if (monthName == "August")
                return 8;
            if (monthName == "Septemper")
                return 9;
            if (monthName == "October")
                return 10;
            if (monthName == "November")
                return 11;
            if (monthName == "December")
                return 12;
            return 0;
        }

        private string GetByMonthDay(ScheduleAppointment app)
        {
            string byMonthDay = "";
            if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
            {
                byMonthDay = app.MonthlyDays.ToString();
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
            {
                byMonthDay = app.YearlyDays.ToString(); 
            }

            if (byMonthDay != "")
            {
                byMonthDay = "BYMONTHDAY=" + byMonthDay + ";";
            }

            return byMonthDay;
        }

        private string GetBySetPos(ScheduleAppointment app)
        {
            string bySetPos = "";
            if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
            {
                if (app.MonthlyWeekOrderSelected == "first")
                    bySetPos = "1";
                if (app.MonthlyWeekOrderSelected == "second")
                    bySetPos = "2";
                if (app.MonthlyWeekOrderSelected == "third")
                    bySetPos = "3";
                if (app.MonthlyWeekOrderSelected == "fourth")
                    bySetPos = "4";
                if (app.MonthlyWeekOrderSelected == "last")
                    bySetPos = "-1";
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
            {
                if (app.YearlyMultiWeekOrderSelected == "first")
                    bySetPos = "1";
                if (app.YearlyMultiWeekOrderSelected == "second")
                    bySetPos = "2";
                if (app.YearlyMultiWeekOrderSelected == "third")
                    bySetPos = "3";
                if (app.YearlyMultiWeekOrderSelected == "fourth")
                    bySetPos = "4";
                if (app.YearlyMultiWeekOrderSelected == "last")
                    bySetPos = "-1";
            }

            if (bySetPos != "")
            {
                bySetPos = "BYSETPOS=" + bySetPos + ";";
            }
            return bySetPos;
        }

        private string GetByDay(ScheduleAppointment app)
        {
            string byDay = "";
            if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
            {
                if (app.IsWeeklySundaySelected == true)
                    byDay = "SU,";
                if (app.IsWeeklyMondaySelected == true)
                    byDay = byDay + "MO,";
                if (app.IsWeeklyTuesdaySelected == true)
                    byDay = byDay + "TU,";
                if (app.IsWeeklyWednesdaySelected == true)
                    byDay = byDay + "WE,";
                if (app.IsWeeklyThursdaySelected == true)
                    byDay = byDay + "TH,";
                if (app.IsWeeklyFridaySelected == true)
                    byDay = byDay + "FR,";
                if (app.IsWeeklySaturdaySelected == true)
                    byDay = byDay + "SA,";               
            }
            else if(app.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
            {
                if (app.MonthlyDaySelected == "Sunday")
                    byDay = "SU,";
                if (app.MonthlyDaySelected == "Monday")
                    byDay = "MO,";
                if (app.MonthlyDaySelected == "Tuesday")
                    byDay = "TU,";
                if (app.MonthlyDaySelected == "Wednesday")
                    byDay = "WE,";
                if (app.MonthlyDaySelected == "Thursday")
                    byDay = "TH,";
                if (app.MonthlyDaySelected == "Friday")
                    byDay = "FR,";
                if (app.MonthlyDaySelected == "Saturday")
                    byDay = "SA,";
                if (app.MonthlyDaySelected == "day")
                    byDay = "SU,MO,TU,WE,TH,FR,SA,";
                if (app.MonthlyDaySelected == "weekday")
                    byDay = "MO,TU,WE,TH,FR,";
                if (app.MonthlyDaySelected == "weekend day")
                    byDay = "SU,SA,"; 
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
            {
                if (app.YearlyMultiDaySelected == "Sunday")
                    byDay = "SU,";
                if (app.YearlyMultiDaySelected == "Monday")
                    byDay = "MO,";
                if (app.YearlyMultiDaySelected == "Tuesday")
                    byDay = "TU,";
                if (app.YearlyMultiDaySelected == "Wednesday")
                    byDay = "WE,";
                if (app.YearlyMultiDaySelected == "Thursday")
                    byDay = "TH,";
                if (app.YearlyMultiDaySelected == "Friday")
                    byDay = "FR,";
                if (app.YearlyMultiDaySelected == "Saturday")
                    byDay = "SA,";
                if (app.YearlyMultiDaySelected == "day")
                    byDay = "SU,MO,TU,WE,TH,FR,SA,";
                if (app.YearlyMultiDaySelected == "weekday")
                    byDay = "MO,TU,WE,TH,FR,";
                if (app.YearlyMultiDaySelected == "weekend day")
                    byDay = "SU,SA,";  
            }

            if (byDay != "")
            {
                byDay = byDay.Substring(0, byDay.Length - 1);
                byDay = "BYDAY=" + byDay + ";";
            }
            return byDay;
        }

        private string GetCount(ScheduleAppointment app)
        {
            int count = 0;
            string countString = "";
            DateTime startTime, endTime;
            if (app.StartRecurrenceTime == DateTime.MinValue)
                startTime = app.StartTime;
            else
                startTime = app.StartRecurrenceTime;

            if (app.EndRecurrenceTime == DateTime.MinValue)
                endTime = app.EndTime;
            else
                endTime = app.EndRecurrenceTime;           

            if (app.IsEndAfter == true)
            {
                count = app.EndOccurenceCount;
            }
            else if (app.IsEndBy == true)
            {
                if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Daily)
                {
                    if (app.IsDailyWeekDays == false)
                    {
                        if (app.IsDailyCustomDays == false)
                        {
                            TimeSpan dateDiff = endTime.Date - startTime.Date;
                            count = dateDiff.Days;
                        }
                        else
                        {                            
                            TimeSpan diff = endTime.Date - startTime.Date;
                            int diffCount = diff.Days;
                            int interval = app.DailyDays;
                            for (int i = 0; i <= diffCount; i = i + interval)
                            {
                                count = count + 1;
                            }
                        }
                    }
                    else
                    {                                            
                        TimeSpan diff = endTime.Date - startTime.Date;
                        int diffCount = diff.Days;                      
                        for (int i = 0; i <= diffCount; i++)
                        {
                            if (startTime.DayOfWeek == DayOfWeek.Monday || startTime.DayOfWeek == DayOfWeek.Tuesday || startTime.DayOfWeek == DayOfWeek.Wednesday || startTime.DayOfWeek == DayOfWeek.Thursday || startTime.DayOfWeek == DayOfWeek.Friday)
                            {
                                count = count + 1;
                            }
                           startTime =  startTime.AddDays(1);
                        }                         
                    }
                }
                else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
                {
                    List<DayOfWeek> dayOfWeekCollection = new List<DayOfWeek>();
                    if (app.IsWeeklySundaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Sunday);
                    if (app.IsWeeklyMondaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Monday);
                    if (app.IsWeeklyTuesdaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Tuesday);
                    if (app.IsWeeklyWednesdaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Wednesday);
                    if (app.IsWeeklyThursdaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Thursday);
                    if (app.IsWeeklyFridaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Friday);
                    if (app.IsWeeklySaturdaySelected == true)
                        dayOfWeekCollection.Add(DayOfWeek.Saturday);
                
                    DateTime wkStDt = DateTime.MinValue;
                    wkStDt = startTime.AddDays(0 - (int)startTime.DayOfWeek);

                    TimeSpan diff = endTime.Date - wkStDt.Date;
                    int diffCount = diff.Days + 1;
                    int interval = app.WeeklyWeeks;

                    if (interval <= 1)
                    {
                        for (int i = 0; i <= diffCount; i = i + 1)
                        {
                            if (wkStDt.Date > endTime.Date)
                                break;
                            if (wkStDt.Date >= startTime.Date)
                            {
                                for (int j = 0; j < dayOfWeekCollection.Count; j++)
                                {
                                    if (wkStDt.DayOfWeek == dayOfWeekCollection[j])
                                    {
                                        count = count + 1;
                                        break;
                                    }
                                }
                            }                          
                            wkStDt = wkStDt.AddDays(1);
                        }
                    }
                    else
                    {
                        int intervalCount = 0;
                        for (int i = 0; i <= diffCount; i = i + 1)
                        {
                            if (wkStDt.Date > endTime.Date)
                                break;
                            if (wkStDt.Date >= startTime.Date)
                            {
                                for (int j = 0; j < dayOfWeekCollection.Count; j++)
                                {
                                    if (wkStDt.DayOfWeek == dayOfWeekCollection[j])
                                    {
                                        count = count + 1;
                                        break;
                                    }
                                }
                            }
                            intervalCount = intervalCount + 1;
                            if (intervalCount == (interval -1 ) * 7)
                            {
                                wkStDt = wkStDt.AddDays(7 * (interval - 1 ));
                                i = i + (7 * (interval -1));
                                intervalCount = 0;
                            }
                            wkStDt = wkStDt.AddDays(1);                            
                        }
 
                    }
                }
                else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
                {
                    if (app.IsMonthlyMultiDays == false)
                    {
                        TimeSpan diff = endTime.Date - startTime.Date;
                        int diffCount = diff.Days + 1;
                        int interval = app.MonthlyMonth;
                        for (int i = 0; i < diffCount; i++)
                        {
                            int customDay = app.MonthlyDays;
                            if (startTime.Date.Day == customDay)
                            {
                                count = count + 1;
                                i = i + 26;
                                startTime = startTime.AddDays(27);
                                continue;
                            }
                            startTime = startTime.AddDays(1);
                        }
                        count = count / interval;
                    }
                    else
                    {
                        TimeSpan diff = endTime.Date - startTime.Date;
                        int diffCount = diff.Days + 1;
                        int interval = app.MonthlyMonthMulti;
                        string monthlyDaySelected = app.MonthlyDaySelected;
                        for (int i = 0; i < diffCount; i++)
                        {
                            if (startTime.Date > endTime.Date)
                                break;
                            List<DateTime> weekDates = this.GetWeekDates(startTime, app.MonthlyWeekOrderSelected);
                            if (app.MonthlyDaySelected == "day")
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == "Sunday")
                                    {
                                        if(startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddMonths(1);
                                        break;
                                    }
                                }
                            }
                            else if (app.MonthlyDaySelected == "weekend day")
                            {
                                 foreach (var date in weekDates)
                                    {
                                        if (date.DayOfWeek.ToString() == "Sunday")
                                        {
                                            if (startTime >= app.StartTime)
                                                count = count + 1;
                                            startTime = startTime.AddMonths(1);
                                            break;
                                        }
                                    }
 
                            }
                            else if(app.MonthlyDaySelected == "weekday")
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == "Monday")
                                    {
                                        if (startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddMonths(1);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == app.MonthlyDaySelected)
                                    {
                                       if(startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddMonths(1);
                                        break;
                                    }
                                }
                            }
                        }
                        count = count / interval;                        
                    }
                }
                else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
                {
                    if (app.IsYearlyMultiDays == false)
                    {
                        TimeSpan diff = endTime.Date - startTime.Date;
                        int diffCount = diff.Days + 1;
                        int interval = app.YearlyYear;
                        int customDay = app.YearlyDays;
                        string customMonth = app.YearlyMonthSelected;
                        int customMonthValue = this.GetByMonthValue(customMonth);    
                        for (int i = 0; i < diffCount; i++)
                        {
                            if (startTime.Date.Day == customDay && startTime.Date.Month == customMonthValue)
                            {
                                count = count + 1;
                                i = i + 363;
                                startTime = startTime.AddDays(364);
                                continue;
                            }
                            startTime = startTime.AddDays(1);
                        }            
                        count = count / interval;
                    }
                    else
                    {
                        TimeSpan diff = endTime.Date - startTime.Date;
                        int diffCount = diff.Days + 1;
                        int interval = app.YearlyYear;
                        string yearlyDaySelected = app.YearlyMultiDaySelected;
                        for (int i = 0; i < diffCount; i++)
                        {

                            if (startTime.Date > endTime.Date)
                                break;
                            List<DateTime> weekDates = this.GetWeekDates(startTime, app.YearlyMultiWeekOrderSelected);
                            if (app.YearlyMultiDaySelected == "day")
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == "Sunday")
                                    {
                                        if (startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddYears(1);
                                        break;
                                    }
                                }
                            }
                            else if (app.YearlyMultiDaySelected == "weekend day")
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == "Sunday")
                                    {
                                        if (startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddYears(1);
                                        break;
                                    }
                                }

                            }
                            else if (app.YearlyMultiDaySelected == "weekday")
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == "Monday")
                                    {
                                        if (startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddYears(1);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (var date in weekDates)
                                {
                                    if (date.DayOfWeek.ToString() == app.YearlyMultiDaySelected)
                                    {
                                        if (startTime >= app.StartTime)
                                            count = count + 1;
                                        startTime = startTime.AddYears(1);
                                        break;
                                    }
                                }
                            }
                        }
                        count = count / interval;                                                                         
                    }
                }
            }

            if (count > 0)
                countString = "COUNT=" + count.ToString() + ";";

            return countString;
        }

        private List<DateTime> GetWeekDates(DateTime currDate, string order)
        {
            List<DateTime> dt = new List<DateTime>();
            if (order == "first")
            {
                for (int i = 0; i <= 6; i++)
                {
                    dt.Add(new DateTime(currDate.Year, currDate.Month, i+1));
                }
                return dt;
            }
            else if (order == "second")
            {
                for (int i = 7; i <= 13; i++)
                {
                    dt.Add(new DateTime(currDate.Year, currDate.Month, i + 1));
                }
                return dt; 
            }
            else if (order == "third")
            {
                for (int i = 14; i <= 20; i++)
                {
                    dt.Add(new DateTime(currDate.Year, currDate.Month, i + 1));
                }
                return dt;
            }
            else if (order == "fourth")
            {
                for (int i = 21; i <= 27; i++)
                {
                    dt.Add(new DateTime(currDate.Year, currDate.Month, i + 1));
                }
                return dt;
            }
            else
            {
                for (int i = DateTime.DaysInMonth(currDate.Year, currDate.Month) - 7; i <= DateTime.DaysInMonth(currDate.Year, currDate.Month) ; i++)
                {
                    dt.Add(new DateTime(currDate.Year, currDate.Month, i + 1 ));
                }
 
            }
            return dt;
        }

        private string GetInterval(ScheduleAppointment app)
        {
            string intervalString = "";
            int interval = 0;
            if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Daily)
            {
                interval = app.DailyDays;
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Weekly)
            {
                interval = app.WeeklyWeeks;
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Monthly)
            {
                if (app.IsMonthlyMultiDays)
                {
                    interval = app.MonthlyMonthMulti;
                }
                else
                {
                    interval = app.MonthlyMonth;
                }
                
            }
            else if (app.CurrentRecurrencePatternMode == RecurrencePatternMode.Yearly)
            {
                interval = app.YearlyYear;             
            }

            if (interval > 1)
                intervalString = "INTERVAL=" + interval.ToString() + ";";

            return intervalString;
        }

    }

    

}
