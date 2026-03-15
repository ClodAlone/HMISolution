#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using Syncfusion.JavaScript.DataSources;
using Syncfusion.JavaScript;


namespace Syncfusion.JavaScript
{
   public class RatingPropertiesBuilder
    {
       public Rating rating;

        public RatingPropertiesBuilder(Rating rating)
        { this.rating = new Rating(rating.ID, rating.RatingModel); }
        
        public RatingPropertiesBuilder()
        {
        }
        //int  Values
        public RatingPropertiesBuilder MinValue(int minValue)
        {
            rating.RatingModel.MinValue = minValue;
            return this;
        }
        public RatingPropertiesBuilder MaxValue(int maxValue)
        {
            rating.RatingModel.MaxValue = maxValue;
            return this;
        }
        public RatingPropertiesBuilder CurrentValue(int currentValue)
        {
            rating.RatingModel.CurrentValue = currentValue;
            return this;
        }
        public RatingPropertiesBuilder ShapeWidth(int shapeWidth)
        {
            rating.RatingModel.ShapeWidth = shapeWidth;
            return this;
        }
        public RatingPropertiesBuilder ShapeHeight(int shapeHeight)
        {
            rating.RatingModel.ShapeHeight = shapeHeight;
            return this;
        }
        public RatingPropertiesBuilder IncrementStep(int incrementStep)
        {
            rating.RatingModel.IncrementStep = incrementStep;
            return this;
        }

        //Boolean values
        public RatingPropertiesBuilder AllowReset()
        {
            rating.RatingModel.AllowReset = true;
            return this;
        }
        public RatingPropertiesBuilder AllowReset(bool allowReset)
        {
            rating.RatingModel.AllowReset = allowReset;
            return this;
        }
        public RatingPropertiesBuilder ReadOnly()
        {
            rating.RatingModel.ReadOnly = true;
            return this;
        }
        public RatingPropertiesBuilder ReadOnly(bool readOnly)
        {
            rating.RatingModel.ReadOnly = readOnly;
            return this;
        }
        public RatingPropertiesBuilder Enabled()
        {
            rating.RatingModel.Enabled = true;
            return this;
        }
        public RatingPropertiesBuilder Enabled(bool enabled)
        {
            rating.RatingModel.Enabled = enabled;
            return this;
        }
        public RatingPropertiesBuilder ShowTooltip()
        {
            rating.RatingModel.ShowTooltip = true;
            return this;
        }
        public RatingPropertiesBuilder ShowTooltip(bool showTooltip)
        {
            rating.RatingModel.ShowTooltip = showTooltip;
            return this;
        }
        public RatingPropertiesBuilder Persist()
        {
            rating.RatingModel.Persist = true;
            return this;
        }
        public RatingPropertiesBuilder Persist(bool persist)
        {
            rating.RatingModel.Persist = persist;
            return this;
        }

        //EnumValues
        public RatingPropertiesBuilder Orientation(Orientation orientation)
        {
            rating.RatingModel.Orientation = orientation;
            return this;
        }
        public RatingPropertiesBuilder Precision(Precisions precision)
        {
            rating.RatingModel.Precision = precision;
            return this;
        }

       //String values
        public RatingPropertiesBuilder Height(String height)
        {
            rating.RatingModel.Height = height;
            return this;
        }
        public RatingPropertiesBuilder Width(String width)
        {
            rating.RatingModel.Width = width;
            return this;
        }
        public RatingPropertiesBuilder CssClass(String cssClass)
        {
            rating.RatingModel.CssClass = cssClass;
            return this;
        }

       //Events
        public RatingPropertiesBuilder ClientSideEvents(Action<RatingClientSideEventsBuilder> clientSideEvents)
        {
            var builder = new RatingClientSideEventsBuilder(this.rating.RatingModel);
            if (clientSideEvents != null)
                clientSideEvents.Invoke(builder);
            return this;
        }
        //Render
        public HtmlString Render()
        {
            return new HtmlString(rating.Render().ToString());
        }
        public override String ToString()
        {

            return Render().ToString();
        }

    }
}
