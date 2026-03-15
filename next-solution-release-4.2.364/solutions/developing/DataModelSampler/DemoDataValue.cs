using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataModelSampler
{
    public class DemoDataValue
    {
        // Methods
        public override string ToString()
        {
            return (this.X + " = " + this.Y.ToString());
        }

        // Properties
        public double Close { get; set; }

        [TypeConverter(typeof(TestDTimeConverter))]
        public DateTime Date { get; set; }

        public double From { get; set; }

        public double High { get; set; }

        public string Label { get; set; }

        public double Low { get; set; }

        public double Open { get; set; }

        public double To { get; set; }

        public int Volume { get; set; }

        public string X { get; set; }

        public double Y { get; set; }

        public double Y2 { get; set; }

        public double Z { get; set; }
    }
}
