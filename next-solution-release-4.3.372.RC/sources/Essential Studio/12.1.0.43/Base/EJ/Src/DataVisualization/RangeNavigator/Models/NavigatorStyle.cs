#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript.Shared;

namespace Syncfusion.JavaScript.DataVisualization.Models
{
    public class NavigatorStyle
    {
     
        #region Field

        private string m_selectedRegionColor= null;
        private string m_unselectedRegionColor = null;
        private string m_thumbColor = null;
        private int m_thumbRadius = 10;
        private string m_background = null;

        private object m_border=null;
        private object m_majorGridLineStyle =null;
        private object m_minorGridLineStyle = null;

        #endregion

        #region Property

        [JsonProperty("selectedRegionColor")]
        [DefaultValue(null)]
        public string SelectedRegionColor
        {
            get { return this.m_selectedRegionColor; }
            set { this.m_selectedRegionColor = value; }
        }
        [JsonProperty("unselectedRegionColor")]
        [DefaultValue(null)]
        public string UnselectedRegionColor
        {
            get { return this.m_unselectedRegionColor; }
            set { this.m_unselectedRegionColor = value; }
        }
        [JsonProperty("thumbColor")]
        [DefaultValue(null)]
        public string ThumbColor
        {
            get { return this.m_thumbColor; }
            set { this.m_thumbColor = value; }
        }
        [JsonProperty("thumbRadius")]
        [DefaultValue(10)]
        public int ThumbRadius
        {
            get { return this.m_thumbRadius; }
            set { this.m_thumbRadius = value; }
        }
        [JsonProperty("background")]
        [DefaultValue(null)]
        public string Background
        {
            get { return this.m_background; }
            set { this.m_background = value; }
        }
        [JsonProperty("border")]
        public object Border
        {
            get { return this.m_border; }
            set { this.m_border = value; }
        }
        [JsonProperty("majorGridLineStyle")]
        public object MajorGridLineStyle
        {
            get { return this.m_majorGridLineStyle; }
            set { this.m_majorGridLineStyle = value; }
        }
        [JsonProperty("minorGridLineStyle")]
        public object MinorGridLineStyle
        {
            get { return this.m_minorGridLineStyle; }
            set { this.m_minorGridLineStyle = value; }
        }
        #endregion

        #region ShouldSerialize
        public bool ShouldSerializeBorder()
        {
            if (Utils.PropertyCompare(Border, new LineStyle()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMajorGridLineStyle()
        {
            if (Utils.PropertyCompare(Border, new GridLineStyle()))
                return true;
            else
                return false;
        }
        public bool ShouldSerializeMinorGridLineStyle()
        {
            if (Utils.PropertyCompare(Border, new GridLineStyle()))
                return true;
            else
                return false;
        }
        #endregion


    }
    public class GridLineStyle
    {
        # region Field
        private string m_color=null;
        private bool m_visible=true;
        #endregion

       # region Property
        [JsonProperty("color")]
        [DefaultValue(null)]
        public string Color
        {
            get { return this.m_color; }
            set { this.m_color = value; }
        }
        [JsonProperty("visible")]
        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.m_visible; }
            set { this.m_visible = value; }
        }

        #endregion
    }
}
