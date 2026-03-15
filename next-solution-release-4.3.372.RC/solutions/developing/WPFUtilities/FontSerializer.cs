using System.Drawing;
using System.Globalization;
using System;

namespace Utilities
{
    public static class FontPropertiesHelper
    {
        public static bool SetProperty(Font font, string fontPrefix)
        {
            if (font == null)
                return false;
            ApplicationPropertiesHelper.SetProperty($"{fontPrefix}FontName", font.Name);
            ApplicationPropertiesHelper.SetProperty($"{fontPrefix}FontSize", font.SizeInPoints.ToString(CultureInfo.InvariantCulture));
            ApplicationPropertiesHelper.SetProperty($"{fontPrefix}FontStyle", font.Style.ToString());
            return true;
        }

        public static Font GetProperty(string fontPrefix)
        {
            string fontName = (string)ApplicationPropertiesHelper.GetProperty($"{fontPrefix}FontName");
            if (fontName == null)
                return null;
            float fontSize;
            FontStyle fontStyle;
            if (!float.TryParse(ApplicationPropertiesHelper.GetProperty($"{fontPrefix}FontSize") as string, NumberStyles.Float, CultureInfo.InvariantCulture, out fontSize))
                return null;
            try
            {
                fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), ApplicationPropertiesHelper.GetProperty($"{fontPrefix}FontStyle") as string);
            }catch (Exception)
            {
                fontStyle = 0; //Regular
            }
            return new Font(fontName, fontSize, fontStyle);
        }
    }
}
