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
using System.ComponentModel;
using Syncfusion.JavaScript.Shared.Serializer;
using System.Drawing;
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public class CaptionBuilder
    {
        private BulletGraphProperties bg_model;
        private Caption caption;

        public CaptionBuilder(Caption options)
        {
            this.caption = options;
        }

        public CaptionBuilder(BulletGraph bullet, Caption captionOptions)
        {
            this.caption = captionOptions;
            this.bg_model = bullet.BulletGraphModel;
            this.bg_model.Caption = captionOptions;
            
        }

        public CaptionBuilder TextAngle(int value)
        {
            this.caption.TextAngle = value;
            return this;
        }

        public CaptionBuilder Location(Action<BulletLocationBuilder> locationOptions)
        {
            var obj = new BulletLocation();
            //  scale.Location = locationOptions;
            var builder = new BulletLocationBuilder(obj, this.caption);
            if (locationOptions != null)
                locationOptions.Invoke(builder);
            return this;
        }

        public CaptionBuilder Text(String value)
        {
            this.caption.Text = value;
            return this;
        }

        public CaptionBuilder Font(Action<BulletFontBuilder> fontOptions)
        {
            var obj = new BulletFont();
            var builder = new BulletFontBuilder(this.caption, obj);
            if (fontOptions != null)
                fontOptions.Invoke(builder);
            return this;
        }

        
        public CaptionBuilder SubTitle(Action<SubTitleBuilder> subOptions)
        {
            var obj = new SubTitle();
            var builder = new SubTitleBuilder(obj,this.caption);
            if (subOptions != null)
                subOptions.Invoke(builder);
            return this;
        }
    }
}
