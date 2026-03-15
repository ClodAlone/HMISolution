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
        public GridPropertiesBuilder<T> Grid<T>(string id) where T : class
        {
            var model = new GridProperties<T>();
            var grid = new Grid<T>(id, model);
            return new GridPropertiesBuilder<T>(grid);
        }
        public Grid<T> Grid<T>(String id, GridProperties<T> model) where T : class
        {
            var grid = new Grid<T>(id, model);
            return grid;
        }
    }
}
