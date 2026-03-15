using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mindscape.WpfElements
{
  /// <summary>
  /// Represents a time of day.
  /// </summary>
  [TypeConverter(typeof(TimeOfDayConverter))]
  public struct TimeOfDay : IComparable<TimeOfDay>
  {
    private readonly int _hour;
    private readonly int _minute;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeOfDay"/> class.
    /// </summary>
    /// <param name="hour">The hour of the day.</param>
    public TimeOfDay(int hour)
      : this(hour, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeOfDay"/> class.
    /// </summary>
    /// <param name="hour">The hour of the day.</param>
    /// <param name="minute">The minute of the hour.</param>
    public TimeOfDay(int hour, int minute)
    {
      _hour = hour;
      _minute = minute;
    }

    /// <summary>
    /// The hour of the <see cref="TimeOfDay"/>.
    /// </summary>
    public int Hour
    {
      get { return _hour; }
    }

    /// <summary>
    /// The minute of the <see cref="TimeOfDay"/>.
    /// </summary>
    public int Minute
    {
      get { return _minute; }
    }

    /// <summary>
    /// The hour of the <see cref="TimeOfDay"/>, considered on the twelve-hour clock
    /// (e.g. 14:30 returns 2).
    /// </summary>
    public int TwelveHourClockHour
    {
      get
      {
        if (_hour == 0)
        {
          return 12;
        }
        if (_hour > 12)
        {
          return _hour - 12;
        }
        return _hour;
      }
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is later than another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>true if first is later than second; otherwise false.</returns>
    public static bool operator >(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) > 0;
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is earlier than another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>true if first is earlier than second; otherwise false.</returns>
    public static bool operator <(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) < 0;
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is earlier than or the same as another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>true if first is earlier than or the same as second; otherwise false.</returns>
    public static bool operator <=(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) <= 0;
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is equal to another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>true if first is equal to second; otherwise false.</returns>
    public static bool operator ==(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) == 0;
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is unequal to another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>false if first is equal to second; otherwise true.</returns>
    public static bool operator !=(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) != 0;
    }

    /// <summary>
    /// Gets whether one <see cref="TimeOfDay"/> is later than or the same as another.
    /// </summary>
    /// <param name="first">The first TimeOfDay to compare.</param>
    /// <param name="second">The second TimeOfDay to compare.</param>
    /// <returns>true if first is later than or the same as second; otherwise false.</returns>
    public static bool operator >=(TimeOfDay first, TimeOfDay second)
    {
      return first.CompareTo(second) >= 0;
    }

    /// <summary>
    /// Tests whether another object is equal to this TimeOfDay.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns>true if the other object is an equal TimeOfDay; otherwise false.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is TimeOfDay))
      {
        return false;
      }
      return CompareTo((TimeOfDay)obj) == 0;
    }

    /// <summary>
    /// Gets a hash code for this object.
    /// </summary>
    /// <returns>A hash code for this object</returns>
    public override int GetHashCode()
    {
      return _hour.GetHashCode() + 29 * _minute.GetHashCode();
    }

    /// <summary>
    /// Gets a string representation of the <see cref="TimeOfDay"/>.
    /// </summary>
    /// <returns>A string representation of the TimeOfDay.</returns>
    public override string ToString()
    {
      String result = _hour + " : ";
      if (_minute < 10)
      {
        result += "0";
      }
      result += _minute;
      return result;
    }

    #region IComparable<TimeOfDay> Members

    /// <summary>
    /// Compares this <see cref="TimeOfDay"/> to another.
    /// </summary>
    /// <param name="other">The TimeOfDay to compare against.</param>
    /// <returns>A negative value if this TimeOfDay occurs before the other; a positive
    /// value if this TimeOfDay occurs after the other; 0 if the two TimeOfDays coincide.</returns>
    public int CompareTo(TimeOfDay other)
    {
      if (Hour == other.Hour)
      {
        return Minute - other.Minute;
      }
      return Hour - other.Hour;
    }

    #endregion
  }
}
