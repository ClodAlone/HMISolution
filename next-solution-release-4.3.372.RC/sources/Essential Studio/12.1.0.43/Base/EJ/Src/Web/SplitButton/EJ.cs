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
        public SplitButtonPropertiesBuilder SplitButton(string id)
        {
            var model = new SplitButtonProperties();
            var splitbutton = new SplitButton(id, model);
            return new SplitButtonPropertiesBuilder(splitbutton);
        }
        public SplitButton SplitButton(String id, SplitButtonProperties model)
        {
            var splitbutton = new SplitButton(id, model);
            return splitbutton;
        }


    }
}