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

namespace Syncfusion.JavaScript.DataVisualization.Models.Collections
{
    [Serializable]
    public class LabelFontStyle
    {
        private bool _bold;

        public bool Bold
        {
            get { return _bold; }
            set { _bold = value; }
        }

        private bool _italic;

        public bool Italic
        {
            get { return _italic; }
            set { _italic = value; }
        }

        private bool _underline;

        public bool Underline
        {
            get { return _underline; }
            set { _underline = value; }
        }

        private bool none;

        public bool None
        {
            get { return none; }
            set { none = value; }
        }

    }
}
