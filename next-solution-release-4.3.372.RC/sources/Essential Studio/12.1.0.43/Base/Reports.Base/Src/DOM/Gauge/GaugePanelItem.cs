//-------------------------------------------------------------------------------------------------
// <copyright file="GaugePanelItem.cs" company="syncfusion">
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
    public class GaugePanelItem
    {
        #region Members

        private string name = string.Empty;

        private ActionInfo actionInfo = null;

        private double top;

        private double left;

        private double height;

        private double width;

        private int zIndex = 0;

        private string tooTlip = string.Empty;

        private bool hidden;

        private string parentItem = string.Empty;

        #endregion

        #region Public Properties

        [XmlAttribute()]    
        public string Name
        {
            get { return name; }

            set { name = value; }
        }

        public double Top
        {
            get { return this.top; }

            set { this.top = value; }
        }

        public double Left
        {
            get { return this.left; }

            set { this.left = value; }
        }

        public double Height
        {
            get { return this.height; }

            set { this.height = value; }
        }

        public double Width
        {
            get { return this.width; }

            set { this.width = value; }
        }

        public int ZIndex
        {
            get { return zIndex; }

            set { zIndex = value; }
        }

        public string ToolTip
        {
            get { return tooTlip; }

            set { tooTlip = value; }
        }

        public ActionInfo ActionInfo
        {
            get { return actionInfo; }

            set { actionInfo = value; }
        }

        public bool Hidden
        {
            get { return hidden; }

            set { hidden = value; }
        }

        public string ParentItem
        {
            get { return parentItem; }

            set { parentItem = value; }
        }

        #endregion

        #region Constructors

        public GaugePanelItem()
        {
        }

        #endregion
    }
}
