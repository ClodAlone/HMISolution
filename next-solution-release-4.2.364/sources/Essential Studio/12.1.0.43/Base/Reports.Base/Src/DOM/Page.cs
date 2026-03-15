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
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
namespace Syncfusion.RDL.DOM
{
    public class Page
    {
        #region Default Page DOM
        private Size m_pageHeight = "11in"; // Default 11in
        private Size m_pageWidth = "8.5in"; // Default 8.5in
        private Size m_leftMargin = "0in"; // Default 0in
        private Size m_rightMargin = "0in"; // Default 0in
        private Size m_topMargin = "0in"; // Default 0in
        private Size m_bottomMargin = "0in"; // Default 0in
        #endregion

        public Size PageHeight
        {
            get
            {
                return this.m_pageHeight;
            }
            set
            {
                this.m_pageHeight = value;
            }
        }

        public Size PageWidth
        {
            get
            {
                return this.m_pageWidth;
            }
            set
            {
                this.m_pageWidth = value;
            }
        }

        public PageHeader PageHeader { get; set; }
        public PageFooter PageFooter { get; set; }
        public Size InteractiveHeight { get; set; }
        public Size InteractiveWidth { get; set; }
        public Size LeftMargin { get; set; }
        public Size RightMargin { get; set; }
        public Size TopMargin { get; set; }
        public Size BottomMargin { get; set; }
        [DefaultValue(0)]
        public int Columns { get; set; }
        public Size ColumnSpacing { get; set; }
        public Style Style { get; set; }
    }
}
