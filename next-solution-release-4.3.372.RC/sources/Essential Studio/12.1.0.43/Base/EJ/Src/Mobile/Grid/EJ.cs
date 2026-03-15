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
using Syncfusion.JavaScript.Mobile.Models;

namespace Syncfusion.JavaScript.Mobile
{
    public partial class EssentialJavaScriptMobile
    {
        public MobileGridPropertiesBuilder<T> Grid<T>(string id) where T : class
        {
            var model = new MobileGridProperties<T>();
            var grid = new MobileGrid<T>(id, model);
            return new MobileGridPropertiesBuilder<T>(grid);
        }
        public MobileGrid<T> Grid<T>(String id, MobileGridProperties<T> model) where T : class
        {
            var grid = new MobileGrid<T>(id, model);
            return grid;
        }
    }
}
