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
using Syncfusion.JavaScript.DataVisualization.Models;

namespace Syncfusion.JavaScript.DataVisualization
{
    public partial class ClientSideEventsBuilder
    {
        private BulletGraphProperties bulletModel;
        public ClientSideEventsBuilder(BulletGraphProperties bulletProp)
        {
            bulletModel = bulletProp;
        }

        //Events
        public ClientSideEventsBuilder DrawTicks(String ticks)
        {
            bulletModel.DrawTicks = ticks;
            return this;
        }

        public ClientSideEventsBuilder DrawLabels(String labels)
        {
            bulletModel.DrawLabels = labels;
            return this;
        }

        public ClientSideEventsBuilder DrawCaption(String caption)
        {
            bulletModel.DrawCaption = caption;
            return this;
        }

        public ClientSideEventsBuilder DrawQualitativeRanges(String ranges)
        {
            bulletModel.DrawQualitativeRanges = ranges;
            return this;
        }

        public ClientSideEventsBuilder DrawFeatureMeasureBar(String featureBar)
        {
            bulletModel.DrawFeatureMeasureBar = featureBar;
            return this;
        }

        public ClientSideEventsBuilder DrawCategory(String category)
        {
            bulletModel.DrawCategory = category;
            return this;
        }

        public ClientSideEventsBuilder DrawComparativeMeasureSymbol(String comparativeMeasure)
        {
            bulletModel.DrawComparativeMeasureSymbol = comparativeMeasure;
            return this;
        }


    }
}
