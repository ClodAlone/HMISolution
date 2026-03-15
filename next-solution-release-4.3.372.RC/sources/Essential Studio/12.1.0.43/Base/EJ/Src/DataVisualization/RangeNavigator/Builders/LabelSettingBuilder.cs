#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization 
{
    public class LabelSettingBuilder
    {
        private LabelSetting  m_labelSetting;
        public LabelSettingBuilder(LabelSetting options)
        {
            this.m_labelSetting = options;
        }
        public LabelSettingBuilder LabelStyles(Action<LabelStylesBuilder> lineStyle)
        {
            var obj = new LabelStyles();
            this.m_labelSetting.LabelStyles = obj;
            var builder = new LabelStylesBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }
        public LabelSettingBuilder HigherLevel(Action<HigherLabelSettingLevelBuilder> higherlevel)
        {
            var obj = new HigherLabelSettingLevel();
            this.m_labelSetting.HigherLevel = obj;
            var builder = new HigherLabelSettingLevelBuilder(obj);
            if (higherlevel != null)
                higherlevel.Invoke(builder);
            return this;
        }
        public LabelSettingBuilder LowerLevel(Action<LowerLabelSettingLevelBuilder> lowerlevel)
        {
            var obj = new LowerLabelSettingLevel();
            this.m_labelSetting.LowerLevel = obj;
            var builder = new LowerLabelSettingLevelBuilder(obj);
            if (lowerlevel != null)
                lowerlevel.Invoke(builder);
            return this;
        }
    }

    public class HigherLabelSettingLevelBuilder
    {
        
        private HigherLabelSettingLevel mLabelSettingLevel;
        public HigherLabelSettingLevelBuilder(HigherLabelSettingLevel options)
        {
            this.mLabelSettingLevel = options;
        }
        public HigherLabelSettingLevelBuilder IntervalType(NavigatorIntervalType m_intervalType)
        {
            this.mLabelSettingLevel.IntervalType = m_intervalType;
            return this;
        }
        public HigherLabelSettingLevelBuilder Position(NavigatorPosition m_position)
        {
            this.mLabelSettingLevel.Position = m_position;
            return this;
        }
        public HigherLabelSettingLevelBuilder Visible(bool visible)
        {
            this.mLabelSettingLevel.Visible = visible;
            return this;
        }
        public HigherLabelSettingLevelBuilder LabelPlacement(string labelPlacement)
        {
            this.mLabelSettingLevel.LabelPlacement  = labelPlacement;
            return this;
        }
        public HigherLabelSettingLevelBuilder Border(Action<LineStyleBuilder> label)
        {
            var obj = new LineStyle();
                 this.mLabelSettingLevel.Border = obj;
                 var builder = new LineStyleBuilder(obj);
                 if (label != null)
                     label.Invoke(builder);
            return this;
            
        }
        public HigherLabelSettingLevelBuilder GridLineStyle(Action<LineStyleBuilder> lineStyle)
        {
            var obj = new LineStyle();
            this.mLabelSettingLevel.GridLineStyle = obj;
            var builder = new LineStyleBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }
        public HigherLabelSettingLevelBuilder LabelStyles(Action<LabelStylesBuilder> lineStyle)
        {
            var obj = new LabelStyles();
            this.mLabelSettingLevel.LabelStyles = obj;
            var builder = new LabelStylesBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }

    }
    public class LowerLabelSettingLevelBuilder
    {
        
        private LowerLabelSettingLevel mLabelSettingLevel;
        public LowerLabelSettingLevelBuilder(LowerLabelSettingLevel options)
        {
            this.mLabelSettingLevel = options;
        }
        public LowerLabelSettingLevelBuilder IntervalType(NavigatorIntervalType m_intervalType)
        {
            this.mLabelSettingLevel.IntervalType = m_intervalType;
            return this;
        }
        public LowerLabelSettingLevelBuilder Position(NavigatorPosition m_position)
        {
            this.mLabelSettingLevel.Position = m_position;
            return this;
        }
        public LowerLabelSettingLevelBuilder Visible(bool visible)
        {
            this.mLabelSettingLevel.Visible = visible;
            return this;
        }
        public LowerLabelSettingLevelBuilder LabelPlacement(string labelPlacement)
        {
            this.mLabelSettingLevel.LabelPlacement  = labelPlacement;
            return this;
        }
        public LowerLabelSettingLevelBuilder Border(Action<LineStyleBuilder> label)
        {
            var obj = new LineStyle();
                 this.mLabelSettingLevel.Border = obj;
                 var builder = new LineStyleBuilder(obj);
                 if (label != null)
                     label.Invoke(builder);
            return this;
            
        }
        public LowerLabelSettingLevelBuilder GridLineStyle(Action<LineStyleBuilder> lineStyle)
        {
            var obj = new LineStyle();
            this.mLabelSettingLevel.GridLineStyle = obj;
            var builder = new LineStyleBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }
        public LowerLabelSettingLevelBuilder LabelStyles(Action<LabelStylesBuilder> lineStyle)
        {
            var obj = new LabelStyles();
            this.mLabelSettingLevel.LabelStyles = obj;
            var builder = new LabelStylesBuilder(obj);
            if (lineStyle != null)
                lineStyle.Invoke(builder);
            return this;

        }

    }
    public class LabelStylesBuilder
    {
        private LabelStyles mLabelStyle;
        public LabelStylesBuilder(LabelStyles options)
        {
            this.mLabelStyle = options;
        }

        public LabelStylesBuilder Font(Action<NavigatorFontBuilder> font)
        {
            var obj = new NavigatorFont();
            this.mLabelStyle.Font = obj;
            var builder = new NavigatorFontBuilder(obj);
            if (font != null)
                font.Invoke(builder);
            return this;
        }
        public LabelStylesBuilder HorizontalAlignment(HorizontalAlignment m_horizontalAlignment)
        {
            this.mLabelStyle.HorizontalAlignment = m_horizontalAlignment;
            return this;
        }

    }
    public class LineStyleBuilder
    {
        private LineStyle mLineStyle;
        public LineStyleBuilder(LineStyle options)
        {
            this.mLineStyle = options;
        }
        public LineStyleBuilder Color(string color)
        {
            this.mLineStyle.Color = color;
            return this;
        }
        public LineStyleBuilder Width(double width)
        {
            this.mLineStyle.Width = width;
            return this;
        }
        public LineStyleBuilder StrokeDashArray(string value)
        {
            this.mLineStyle.StrokeDashArray = value;
            return this;
        }
    }

   
}
