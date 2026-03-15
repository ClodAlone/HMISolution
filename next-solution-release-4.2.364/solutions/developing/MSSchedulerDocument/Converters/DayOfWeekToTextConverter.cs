using MSModel;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Data;
using WPFUtilities;
using System.Windows.Media;
using System.Collections.Generic;
using TranslationHelpers;

namespace MSSchedulerSettings.Converters
{
    public class DayOfWeekToTextConverter : IValueConverter //IMultiValueConverter
    {
        public string StringPlaceolder { get; set; }
        public IDictionary<String, String> Stringlist { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DayOfWeek))
                return Binding.DoNothing;
            try
            {
                if(parameter is bool)
                    switch ((DayOfWeek)value)
                    {
                        case DayOfWeek.Sunday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortSunday", Stringlist, Properties.Resources.NewEventShortSunday);
                        case DayOfWeek.Monday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortMonday", Stringlist, Properties.Resources.NewEventShortMonday);
                        case DayOfWeek.Tuesday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortTuesday", Stringlist, Properties.Resources.NewEventShortTuesday);
                        case DayOfWeek.Wednesday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortWednesday", Stringlist, Properties.Resources.NewEventShortWednesday);
                        case DayOfWeek.Thursday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortThursday", Stringlist, Properties.Resources.NewEventShortThursday);
                        case DayOfWeek.Friday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortFriday", Stringlist, Properties.Resources.NewEventShortFriday);
                        case DayOfWeek.Saturday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortSaturday", Stringlist, Properties.Resources.NewEventShortSaturday);
                        default:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventShortSunday", Stringlist, Properties.Resources.NewEventShortSunday);
                    }
                else 
                    switch ((DayOfWeek)value)
                    {
                        case DayOfWeek.Sunday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventSunday", Stringlist, Properties.Resources.NewEventSunday);
                        case DayOfWeek.Monday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventMonday", Stringlist, Properties.Resources.NewEventMonday); 
                        case DayOfWeek.Tuesday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventTuesday", Stringlist, Properties.Resources.NewEventTuesday); 
                        case DayOfWeek.Wednesday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventWednesday", Stringlist, Properties.Resources.NewEventWednesday); 
                        case DayOfWeek.Thursday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventThursday", Stringlist, Properties.Resources.NewEventThursday);
                        case DayOfWeek.Friday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventFriday", Stringlist, Properties.Resources.NewEventFriday);
                        case DayOfWeek.Saturday:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventSaturday", Stringlist, Properties.Resources.NewEventSaturday); 
                        default:
                            return TranslationHelper.TranlslateText($"_{StringPlaceolder}_NewEventSunday", Stringlist, Properties.Resources.NewEventSunday);
                    }
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
