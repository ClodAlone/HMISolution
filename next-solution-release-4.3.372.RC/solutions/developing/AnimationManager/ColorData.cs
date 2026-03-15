using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_UWP
using System.Windows.Media;
#else
using Windows.UI;
#endif

namespace AnimationManager
{
    [DataContract(Name = "ColorData")]
    public class ColorData : IComparable
    {
        [DataMember]
        double valueColor;
        [DataMember]
        Color color;
        [DataMember]
        Color colorBlink;
        [DataMember]
        int blinkTime;
        [DataMember]
        String text;

#region Properties
        public double Value
        {
            get { return valueColor; }
            set
            {
                valueColor = value;
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }

        public Color BlinkColor
        {
            get { return colorBlink; }
            set
            {
                colorBlink = value;
            }
        }

        public String Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }

        public int BlinkTime
        {
            get { return blinkTime; }
            set
            {
                blinkTime = value;
            }
        }
#endregion

        public int CompareTo(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
            {
                return +1;
            }

            if (Object.ReferenceEquals(obj, this))
            {
                return 0;
            }

            var colorData = obj as ColorData;

            if (colorData == null)
            {
                return -1;
            }

            return Value > colorData.Value ? +1 : -1;
        }
    }
}
