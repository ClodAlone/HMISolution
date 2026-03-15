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
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class BulletFontBuilder
    {
        private BulletLabels label_model;
        private Caption caption_model;
        private SubTitle subTitle_model;

        private BulletFont font; 

        public BulletFontBuilder(BulletLabels label, BulletFont fontOptions)
        {
            this.font = fontOptions;
            this.label_model = label;
            this.label_model.Font = fontOptions;
            
        }

        public BulletFontBuilder(Caption caption, BulletFont fontOptions)
        {
            this.font = fontOptions;
            this.caption_model = caption;
            this.caption_model.Font = fontOptions;

        }

        public BulletFontBuilder(SubTitle sub_Options, BulletFont fontOptions)
        {
            this.font = fontOptions;
            this.subTitle_model = sub_Options;
            this.subTitle_model.Font = fontOptions;

        }

        public BulletFontBuilder FontColor(Color f_Color)
        {
            this.font.FontColor = Convert.ToString(f_Color.Name);
            return this;
        }

        public BulletFontBuilder BulletFontFamily(String f_Family)
        {
            this.font.FontFamily = f_Family;
            return this;
        }

        public BulletFontBuilder BulletFontStyle(String f_Style)
        {
            this.font.FontStyle = f_Style;
            return this;
        }

        public BulletFontBuilder FontSize(String size)
        {
            this.font.Size = size;
            return this;
        }

        public BulletFontBuilder FontWeight(String f_Weight)
        {
            this.font.FontWeight = f_Weight;
            return this;
        }

        public BulletFontBuilder Opacity(double f_Opacity)
        {
            this.font.Opacity = f_Opacity;
            return this;
        }

        
    }
}
