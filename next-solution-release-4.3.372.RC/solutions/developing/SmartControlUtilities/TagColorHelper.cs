using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SmartControlUtilities
{
    public static class TagColorHelper
    {
        #region Method
        public static Brush RandomBrush()
        {
            return new SolidColorBrush(RandomColor());
        }

        public static Color RandomColor()
        {
            System.Reflection.PropertyInfo[] brushInfo = typeof(Colors).GetProperties();
            Color[] brushList = new Color[brushInfo.Length];
            for (int i = 0; i < brushInfo.Length; i++)
            {
                brushList[i] = (Color)brushInfo[i].GetValue(null, null);
            }
            Random randomNumber = new Random(DateTime.Now.Second);
            return brushList[randomNumber.Next(1, brushList.Length)];
        }
        #endregion
    }
}
