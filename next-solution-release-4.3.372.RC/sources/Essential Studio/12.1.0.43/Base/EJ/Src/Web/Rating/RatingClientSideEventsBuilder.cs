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
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class RatingClientSideEventsBuilder
    {
        private RatingProperties ratingModel;
        public RatingClientSideEventsBuilder(RatingProperties ratingProp)
        {
            ratingModel = ratingProp;
        }
        //Events
        public RatingClientSideEventsBuilder Create(String create)
        {
            ratingModel.Create = create;
            return this;
        }
        public RatingClientSideEventsBuilder Click(String click)
        {
            ratingModel.Click = click;
            return this;
        }
         public RatingClientSideEventsBuilder MouseOver(String mouseOver)
        {
            ratingModel.MouseOver = mouseOver;
            return this;
        }
         public RatingClientSideEventsBuilder MouseOut(String mouseOut)
        {
            ratingModel.MouseOut = mouseOut;
            return this;
        }
         public RatingClientSideEventsBuilder ValueChanged(String valueChanged)
        {
            ratingModel.ValueChanged = valueChanged;
            return this;
        }
        public RatingClientSideEventsBuilder Destroy(String destroy)
        {
            ratingModel.Destroy = destroy;
            return this;
        }
    }
}
