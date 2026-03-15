#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class Border
    {
        private string m_color = "Black"; // Default: Black (except within ChartDataPoint and ChartSeries, where the default is to use the palette colors).

        [DefaultValue("Black")]
        public string Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public string Style { get; set; }

        public bool ShouldSerializeStyle()
        {
            return this.Style != null && 
                !this.Style.Equals("None",StringComparison.CurrentCultureIgnoreCase) &&
                !this.Style.Equals("Default", StringComparison.CurrentCultureIgnoreCase);
        }

        public void ResetStyle()
        {
            this.Style = DOM.BorderStyles.None.ToString();
        }

        public Size Width { get; set; }

        public bool ShouldSerializeWidth()
        {
            return this.Width != null &&
                !this.Width.size.Equals("1pt", StringComparison.CurrentCultureIgnoreCase);
        }

        public void ResetWidth()
        {
            this.Width = new Size("1pt");
        }
    }


    public abstract class SideBorder
    {
        public string Color { get; set; }
        public string Style { get; set; }
        public Size Width { get; set; }
    }

    public class TopBorder : SideBorder
    {

    }
    
    public class LeftBorder : SideBorder
    {

    }

    public class RightBorder : SideBorder
    {

    }

    public class BottomBorder : SideBorder
    {

    }
}
