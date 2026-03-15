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
using System.Web;
using Syncfusion.JavaScript.Models;

namespace Syncfusion.JavaScript
{
    public class SizeOptionsBuilder 
    {
        private SizeOptions editOption = new SizeOptions();

        public SizeOptionsBuilder(SizeOptions edit)
        {
            editOption = edit;
        }
        public SizeOptionsBuilder Height(String height)
        {
            editOption.Height = height;
            return this;
        }

        public SizeOptionsBuilder Width(String width)
        {
            editOption.Width = width;
            return this;
        }

    }
}