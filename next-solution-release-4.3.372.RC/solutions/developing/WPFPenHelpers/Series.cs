using System;
using System.Collections.Generic;

namespace WPFPenHelpers
{
    public class GenericSeries
    {
        public SerieData pen;
        public int penIndex;
        public List<GenericSeriesPoint> Points;
        public GenericSeries(SerieData pen, int penIndex)
        {
            this.pen = pen;
            this.penIndex = penIndex;
            this.Points = new List<GenericSeriesPoint>();
        }
    }
    public class GenericSeriesPoint
    {
        public DateTime argument;
        public double value;
        public double secondvalue;
        public GenericSeriesPoint(DateTime argument, double value, double secondvalue)
        {
            this.argument = argument;
            this.value = value;
            this.secondvalue = secondvalue;
        }
    }
}
