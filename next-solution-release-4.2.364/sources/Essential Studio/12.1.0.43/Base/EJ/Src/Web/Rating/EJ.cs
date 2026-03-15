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
using Syncfusion.JavaScript;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public partial class EssentialJavaScript
    {
        public RatingPropertiesBuilder Rating(string id)
        {
            var model = new RatingProperties();
            var rating = new Rating(id, model);
            return new RatingPropertiesBuilder(rating);
        }
        public Rating Rating(String id, RatingProperties model)
        {
            var rating = new Rating(id, model);
            return rating;
        }


    }
}
