using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
#if SILVERLIGHT
using Mindscape.SilverlightElements.Internal;
#else
using Mindscape.WpfElements.Internal;
#endif

#if SILVERLIGHT
namespace Mindscape.SilverlightElements
#else
namespace Mindscape.WpfElements
#endif
{
  partial class SchedulerCanvas
  {
    private class SchedulerElementManager
    {
      private Dictionary<SchedulerElement, SchedulerElementPlacementInfo> _elements;
      private IList<Spring> _springs;
      private int _maxCount;

      public SchedulerElementManager()
      {
        _elements = new Dictionary<SchedulerElement, SchedulerElementPlacementInfo>();// new List<AppointmentElement>();
        _springs = new List<Spring>();
      }

      public double Left { get; set; }

      public double Right { get; set; }

      public double HourSlotHeight { get; set; }

      public void AddElement(SchedulerElement element)
      {
        if (element == null)
        {
          throw new ArgumentNullException("element", "Can not add a null element");   //Just a precaution for now. Might remove this.
        }
        SchedulerElementPlacementInfo info = new SchedulerElementPlacementInfo();
        info.MaxSpan = int.MaxValue;
        _elements.Add(element, info);
        element.ScheduleItem.EndTimeChanged += Item_EndTimeChanged;
        element.ScheduleItem.StartTimeChanged += Item_StartTimeChanged;

        ConfigureElementSprings(element);
      }

      public SchedulerElement FindSchedulerElement(ScheduleItem item)
      {
        foreach (SchedulerElement element in _elements.Keys)
        {
          if (element.ScheduleItem == item)
          {
            return element;
          }
        }
        return null;
      }

      private void ConfigureElementSprings(SchedulerElement element)
      {
        //DayModel day = element.FirstVisibleDay; //TODO: make this private global?, it is always going to be the same within one AppointmentElementManager.
        //ScheduleItem appointment = element.ScheduleItem;
        TimeOfDay startTime = GetStartTime(element);
        TimeOfDay endTime = GetEndTime(element);
        Spring startSpring = FindSpring(startTime);
        if (startSpring == null)
        {
          startSpring = new Spring(startTime);
          InsertSpring(startSpring);
        }
        Spring endSpring = FindSpring(endTime);
        if (endSpring == null)
        {
          endSpring = new Spring(endTime);
          InsertSpring(endSpring);
        }
        IList<Spring> springs = FindSpringsAcross(startTime, endTime);

        int elementIndex = 0;
        for (int index = 0; index < _elements.Count + 1; index++)
        {
          bool clear = true;
          foreach (Spring spring in springs)
          {
            IList<SchedulerElement> elements = spring.SchedulerElements;
            if (index < elements.Count && spring.SchedulerElements[index] != null)
            {
              clear = false;
              break;
            }
          }
          if (clear)
          {
            elementIndex = index;
            break;
          }
        }
        SchedulerElementPlacementInfo info = _elements[element];
        info.Index = elementIndex;
        info.MaxSpan = int.MaxValue;
        foreach (Spring spring in springs)
        {
          spring.AddElement(element, _elements, elementIndex);
          if (spring.Count > _maxCount)
          {
            _maxCount = spring.Count;
          }
        }
      }

      private void Item_StartTimeChanged(object sender, EventArgs e)
      {
        _maxCount = 0;
        _springs = new List<Spring>();
        foreach (SchedulerElement element in _elements.Keys)
        {
          ConfigureElementSprings(element);
        }
      }

      private void Item_EndTimeChanged(object sender, EventArgs e)
      {
        _maxCount = 0;
        _springs = new List<Spring>();
        foreach (SchedulerElement element in _elements.Keys)
        {
          ConfigureElementSprings(element);
        }
      }

      private TimeOfDay GetStartTime(SchedulerElement element)
      {
        TimeOfDay startTime = new TimeOfDay(element.ScheduleItem.StartTime.Hour, element.ScheduleItem.StartTime.Minute + 1);
        if (!DateTimeUtils.IsSameDay(element.FirstVisibleDay.Date, element.ScheduleItem.StartTime))
        {
          startTime = new TimeOfDay(0, 0);
        }
        return startTime;
      }

      private TimeOfDay GetEndTime(SchedulerElement element)
      {
        TimeOfDay endTime = new TimeOfDay(element.ScheduleItem.EndTime.Hour, element.ScheduleItem.EndTime.Minute - 1);
        if (endTime.Minute == -1)
        {
          endTime = new TimeOfDay(element.ScheduleItem.EndTime.Hour - 1, 59);
        }
        if (!DateTimeUtils.IsSameDay(element.FirstVisibleDay.Date, element.ScheduleItem.EndTime))
        {
          endTime = new TimeOfDay(23, 59);
        }
        return endTime;
      }

      public void RemoveElement(SchedulerElement element)
      {
        _elements.Remove(element);
        _maxCount = 0;
        _springs = new List<Spring>();
        foreach (SchedulerElement e in _elements.Keys)
        {
          ConfigureElementSprings(e);
        }
        ConfigureElementPositions();
      }

      public SchedulerElement RemoveItem(ScheduleItem appointment)
      {
        foreach (SchedulerElement element in _elements.Keys)
        {
          if (element.ScheduleItem == appointment)
          {
            RemoveElement(element);
            return element;
          }
        }
        return null;
        //throw new Exception("AppointmentElement could not be found.");
      }

      public void FormatScheduleItems(StyleSelector styleSelector, Style defaultStyle)
      {
        Style style = null;
        foreach (SchedulerElement element in _elements.Keys)
        {
          style = null;
          if (styleSelector != null)
          {
            style = styleSelector.SelectStyle(element.ScheduleItem, element);
          }
          element.Style = style ?? defaultStyle;
        }
      }

      public void FormatScheduleItems(DataTemplateSelector templateSelector, DataTemplate defaultTemplate)
      {
        DataTemplate template = null;
        foreach (SchedulerElement element in _elements.Keys)
        {
          template = null;
          if (templateSelector != null)
          {
            template = templateSelector.SelectTemplate(element.ScheduleItem, element);
          }
          element.ContentTemplate = template ?? defaultTemplate;
        }
      }

      public void ConfigureElementPositions()
      {
        if (_maxCount == 0)
        {
          _maxCount = 1;
        }
        double width = (Right - Left) / _maxCount;
        foreach (SchedulerElement element in _elements.Keys)
        {
          SchedulerElementPlacementInfo info = _elements[element];
          ConfigureElementHeight(element);
          int span = info.MaxSpan;
          if (span == int.MaxValue)
          {
            span = _maxCount - info.Index;
          }
          element.Width = width * span;
          Canvas.SetLeft(element, Left + info.Index * width);
        }
      }

      private Spring FindSpring(TimeOfDay time)
      {
        foreach (Spring spring in _springs)
        {
          if (time.Hour == spring.Time.Hour && time.Minute == spring.Time.Minute)
          {
            return spring;
          }
          if (time < spring.Time)
          {
            return null;
          }
        }
        return null;
      }

      private void InsertSpring(Spring spring)
      {
        //List<SchedulerElement> elements = new List<SchedulerElement>();
        int springIndex = 0;
        foreach (Spring s in _springs)
        {
          int index = 0;
          TimeOfDay time = s.Time;
          if (time > spring.Time)
          {
            break;
          }
          foreach (SchedulerElement element in s.SchedulerElements)
          {
            if (element != null)
            {
              TimeOfDay endTime = new TimeOfDay(element.ScheduleItem.EndTime.Hour, element.ScheduleItem.EndTime.Minute);
              if (!DateTimeUtils.IsSameDay(element.FirstVisibleDay.Date, element.ScheduleItem.EndTime))
              {
                endTime = new TimeOfDay(23, 59);
              }
              if (endTime > spring.Time)
              {
                spring.AddElement(element, _elements, index);
                if (spring.Count > _maxCount)
                {
                  _maxCount = spring.Count;
                }
              }
            }
            index++;
          }
          springIndex++;
        }
        _springs.Insert(springIndex, spring);
      }

      private void RemoveSpring(Spring spring)
      {
        _springs.Remove(spring);
      }

      private IList<Spring> FindSpringsAcross(TimeOfDay startTime, TimeOfDay endTime)
      {
        IList<Spring> springs = new List<Spring>();
        foreach (Spring spring in _springs)
        {
          if (spring.Time > endTime) { break; }
          if (spring.Time >= startTime)
          {
            springs.Add(spring);
          }
        }
        return springs;
      }

      private void ConfigureElementHeight(SchedulerElement element)
      {
        element.Height = 50;
        ScheduleItem item = element.ScheduleItem;
        DateTime start = item.StartTime;
        DateTime end = item.EndTime;
        DateTime date = element.FirstVisibleDay.Date;
        if (!DateTimeUtils.IsSameDay(start, date))
        {
          start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }
        if (!DateTimeUtils.IsSameDay(end, date))
        {
          end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 0);
        }
        double top = FindPosition(start);
        Canvas.SetTop(element, top);
        element.Height = Math.Max(0, (FindPosition(end) - top));
      }

      private double FindPosition(DateTime date)
      {
        double position = date.Hour * HourSlotHeight + date.Minute * (HourSlotHeight / 60); //TODO: the first 2 '60' values need to be variable. That is the HourSlotHeight.
        if (date.Hour == 23 && date.Minute == 59)
        {
          position -= 2;
        }
        return position;
      }

      private class Spring
      {
        private IList<SchedulerElement> _elements;
        private TimeOfDay _time;

        public Spring(TimeOfDay time)
        {
          _elements = new List<SchedulerElement>();
          _time = time;
        }

        public void AddElement(SchedulerElement element, Dictionary<SchedulerElement, SchedulerElementPlacementInfo> infoMap, int index)
        {
          while (index >= _elements.Count)
          {
            _elements.Add(null);
          }
          _elements[index] = element;

          SchedulerElement lastElement = null;
          int lastCount = 0;
          for (int i = 0; i < _elements.Count; i++)
          {
            SchedulerElement elem = _elements[i];
            if (elem != null)
            {
              if (lastElement != null)
              {
                SchedulerElementPlacementInfo info = infoMap[lastElement];
                info.MaxSpan = Math.Min(info.MaxSpan, lastCount + 1);
              }
              lastCount = 0;
              lastElement = elem;
            }
            else
            {
              lastCount++;
            }
          }
        }

        public void RemoveElement(SchedulerElement element)
        {
          _elements.Remove(element);
        }

        public TimeOfDay Time
        {
          get { return _time; }
        }

        public int Count { get { return _elements.Count; } } //Includes empty spaces

        public IList<SchedulerElement> SchedulerElements
        {
          get { return _elements; }
        }

        public override String ToString() //For debugging
        {
          String result = Time + ":  ";
          foreach (SchedulerElement element in _elements)
          {
            if (element != null)
            {
              string id = element.ScheduleItem.Name.Substring(element.ScheduleItem.Name.Length - 1, 1);
              result += id + " ";
            }
            else { result += "0 "; }
          }
          return result;
        }
      }

      private class SchedulerElementPlacementInfo
      {
        public int Index { get; set; }
        public int MaxSpan { get; set; }
      }
    }
  }
}
