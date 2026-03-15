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
    public class SubTitleBuilder
    {
        private SubTitle subtitle;
        private Caption caption;

        public SubTitleBuilder(SubTitle options)
        {
            this.subtitle = options;
        }

        public SubTitleBuilder(SubTitle options, Caption caption_Model)
        {
            this.subtitle = options;
            this.caption = caption_Model;
            this.caption.SubTitle = options;
        }
        public SubTitleBuilder TextAngle(int value)
        {
            this.subtitle.TextAngle = value;
            return this;
        }

        public SubTitleBuilder Text(String value)
        {
            this.subtitle.Text = value;
            return this;
        }

        public SubTitleBuilder Font(Action<BulletFontBuilder> fontOptions)
        {
            var obj = new BulletFont();
            var builder = new BulletFontBuilder(this.subtitle, obj);
            if (fontOptions != null)
                fontOptions.Invoke(builder);
            return this;
        }

        public SubTitleBuilder Location(Action<BulletLocationBuilder> locationOptions)
        {
            var obj = new BulletLocation();
            //  scale.Location = locationOptions;
            var builder = new BulletLocationBuilder(obj, this.subtitle);
            if (locationOptions != null)
                locationOptions.Invoke(builder);
            return this;
        }

    }
}
