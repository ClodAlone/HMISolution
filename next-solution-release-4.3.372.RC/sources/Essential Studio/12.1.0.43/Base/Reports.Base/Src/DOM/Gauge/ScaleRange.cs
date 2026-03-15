//-------------------------------------------------------------------------------------------------
// <copyright file="ScaleRange.cs" company="syncfusion">
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
using System.ComponentModel;

namespace Syncfusion.RDL.DOM
{
    public class ScaleRange
    {
        #region public properties

        [XmlAttribute()]
        public string Name
        {
            get;
            set;
        }

        public Style Style
        {
            get;
            set;
        }
        [DefaultValue(BackgroundGradientTypes.Default)]       
        public BackgroundGradientTypes BackgroundGradientType
        {
            get;
            set;
        }

        public float DistanceFromScale
        {
            get;
            set;
        }

        public StartValue StartValue
        {
            get;
            set;
        }

        public EndValue EndValue
        {
            get;
            set;
        }

        public float StartWidth
        {
            get;
            set;
        }

        public float EndWidth
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string InRangeBarPointerColor
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string InRangeLabelColor
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string InRangeTickMarksColor
        {
            get;
            set;
        }

        public Placement Placement
        {
            get;
            set;
        }

        public string ToolTip
        {
            get;
            set;
        }

        public ActionInfo ActionInfo
        {
            get;
            set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        #endregion

        #region Constructors

        public ScaleRange()
        {
        }

        #endregion
    }
}
