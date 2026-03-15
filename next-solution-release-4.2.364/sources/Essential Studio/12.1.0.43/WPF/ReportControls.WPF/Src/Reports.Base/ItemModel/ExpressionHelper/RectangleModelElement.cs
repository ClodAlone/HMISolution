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
using Syncfusion.RDL.DOM;

namespace Syncfusion.RDL.ItemModel
{
    class RectangleItemExpVal
    {
        public string ToolTip
        {
            get;
            set;
        }

        public string BookMark
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        public int Zindex
        {
            get;
            set;
        }

        public string RepeatWidth
        {
            get;
            set;
        }

        public BorderExpval Border
        {
            get;
            set;
        }

        public string BackgroundColor
        {
            get;
            set;
        }

        public BackGroundImageExpVal BackgroundImage
        {
            get;
            set;
        }

        public string DocumentMapLabel
        {
            get;
            set;
        }

    }

    class SubReportItemExpVal : RectangleItemExpVal
    {

    }
}
