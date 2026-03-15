//-------------------------------------------------------------------------------------------------
// <copyright file="GaugeMember.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class GaugeMember
    {
        private GaugeMember gaugeMember = null;

        private Group group;

        private SortExpressions sortExpressions = null;

        public GaugeMember()
        {
        }

        public GaugeMember GaugeMember1
        {
            get { return gaugeMember; }

            set { gaugeMember = value; }
        }

        public Group Group
        {
            get { return group; }

            set { group = value; }
        }

        public SortExpressions SortExpressions
        {
            get { return sortExpressions; }

            set { sortExpressions = value; }
        }
    }
}
