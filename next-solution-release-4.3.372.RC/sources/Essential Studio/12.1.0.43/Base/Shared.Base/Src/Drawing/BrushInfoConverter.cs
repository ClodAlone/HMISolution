#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;

namespace Syncfusion.Drawing
{


    /// <summary>
    ///      Provides a way to convert <see cref="BrushInfo"/> to a string and from a string.
    /// </summary>
    public sealed class BrushInfoConverter: TypeConverter
    {
		/// <override/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)  
        {
            if (sourceType == typeof(string))
                return true;
	        	   
            return base.CanConvertFrom(context,sourceType);
        }

		/// <override/>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)  
        {
            if (value is string)
            {
                string stringValue = (string)value;
                return BrushInfo.Parse(stringValue);
            }

            return base.ConvertFrom(context,culture,value);
        }

		/// <override/>
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)  
        {
            Color foreColor = Color.Empty;
            Color backColor = Color.Empty;
            PatternStyle hatchStyle = PatternStyle.None;
            GradientStyle gradientStyle = GradientStyle.None;
            BrushStyle style = BrushStyle.None;
			BrushInfoColorArrayList colors = null;

            if (propertyValues.Contains("Style"))
                style = (BrushStyle) propertyValues["Style"];
            if (propertyValues.Contains("PatternStyle"))
                hatchStyle = (PatternStyle) propertyValues["PatternStyle"];
            if (propertyValues.Contains("GradientStyle"))
                gradientStyle = (GradientStyle) propertyValues["GradientStyle"];
            if (propertyValues.Contains("BackColor"))
                backColor = (Color) propertyValues["BackColor"];
            if (propertyValues.Contains("ForeColor"))
                foreColor = (Color) propertyValues["ForeColor"];
			if (propertyValues.Contains("GradientColors"))
			{
				BrushInfoColorArrayList temp = (BrushInfoColorArrayList)propertyValues["GradientColors"];
				// Work with a cloned copy, the old reference could be referring to the original list.
				colors = new BrushInfoColorArrayList();
				foreach(Color color in temp)
					colors.Add(color);

				while(colors.Count < 2)
					colors.Add(Color.Empty);

				Color origForeColor = Color.Empty;
				Color origBackColor = Color.Empty;
				BrushInfo old = context.PropertyDescriptor.GetValue(context.Instance) as BrushInfo;
				if(old != null)
				{
					origBackColor = old.BackColor;
					origForeColor = old.ForeColor;
					// In case the BackColor or ForeColor was updated, then udpate the corresponding entries.
					if(backColor != origBackColor)
						colors[0] = backColor;
					if(foreColor != origForeColor)
						colors[colors.Count - 1] = foreColor;
				}

			}
			else
			{
				colors = new BrushInfoColorArrayList();
				colors.Add(backColor);
				colors.Add(foreColor);
			}


            if (context != null && (style == BrushStyle.Solid || style == BrushStyle.None))
            {
               //Trace.WriteLine("Context: " + context.ToString());
                // Adjust Style setting if user has changed pattern or backcolor
                object o = context.PropertyDescriptor.GetValue(context.Instance);
                if (o is BrushInfo)
                {
                    BrushInfo oldBrush = (BrushInfo) o;
                    if (oldBrush.Style == style)
                    {
                        if (hatchStyle != PatternStyle.None)
                            style = BrushStyle.Pattern;

                        else if (gradientStyle != GradientStyle.None)
                            style = BrushStyle.Gradient;

                        else if (!backColor.IsEmpty && foreColor != oldBrush.BackColor)
                            style = BrushStyle.Solid;
                    }

                }
            }

            BrushInfo br = null;
            switch (style)
            {
            case BrushStyle.None:
                br = new BrushInfo();
                break;

            case BrushStyle.Solid:
                br = new BrushInfo(backColor);
                break;

            case BrushStyle.Pattern:
                br = new BrushInfo(hatchStyle, colors);
                break;

            case BrushStyle.Gradient:
                br = new BrushInfo(gradientStyle, colors);
                break;
            }

            return br;
        }

		/// <override/>
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)  
        {
            return true;
        }

		/// <override/>
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)  
        {
            System.ComponentModel.PropertyDescriptorCollection pds
                = TypeDescriptor.GetProperties(typeof(BrushInfo), attributes);

            string[] atts = new string[]
            {
                "Style",
                "BackColor",
                "ForeColor",
                "PatternStyle",
                "GradientStyle",
				"GradientColors"
            };

            if (context != null) 
            {
                PropertyDescriptor[] properties = null;
            
				BrushStyle brushStyle = BrushStyle.None;
				if (value is BrushInfo)
					brushStyle = ((BrushInfo) value).Style;

                // Changing a property base on value is ok, but changing the number of properties
                // will cause an Assert in IDE.

                switch (brushStyle)
                {
                case BrushStyle.None:
                    properties = new PropertyDescriptor[] 
                    {
                        pds.Find("Style", false),
                        pds.Find("BackColor", false),   
                        pds.Find("ForeColor", false),
                        pds.Find("PatternStyle", false),
						pds.Find("GradientColors", false),
                    };                
                    break;

                case BrushStyle.Solid:
                    properties = new PropertyDescriptor[] 
                    {
                        pds.Find("Style", false),
                        pds.Find("BackColor", false),
                        pds.Find("ForeColor", false),
                        pds.Find("PatternStyle", false),
						pds.Find("GradientColors", false),
					};                
                    break;
                            
                case BrushStyle.Pattern:
                    properties = new PropertyDescriptor[] 
                    {
                        pds.Find("Style", false),
                        pds.Find("BackColor", false),
                        pds.Find("ForeColor", false),
                        pds.Find("PatternStyle", false),
						pds.Find("GradientColors", false),
					};                
                    break;
                            
                case BrushStyle.Gradient:
                    properties = new PropertyDescriptor[] 
                    {
                        pds.Find("Style", false),
                        pds.Find("BackColor", false),
                        pds.Find("ForeColor", false),
                        pds.Find("GradientStyle", false),
						pds.Find("GradientColors", false),
					};                
                    break;
                }

                return new PropertyDescriptorCollection(properties).Sort(atts);
            }

            return pds.Sort(atts);
        }

		/// <override/>
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)  
        {
            return true;
        }
		
    }
}
