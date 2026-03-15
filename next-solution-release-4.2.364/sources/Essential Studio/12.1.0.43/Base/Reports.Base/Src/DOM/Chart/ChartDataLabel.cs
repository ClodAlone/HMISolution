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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class ChartDataLabel
    {
        public Style Style { get; set; }
        [DefaultValue(false)]
        public bool UseValueAsLabel { get; set; }
        public string Label { get; set; }
        [DefaultValue(false)]
        public bool Visible { get; set; }
        [DefaultValue(Position.Default)]
        public Position Position { get; set; }
        public int Rotation { get; set; }
        public string ToolTip { get; set; }
        public ActionInfo ActionInfo { get; set; }
    }
}
