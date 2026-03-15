//-------------------------------------------------------------------------------------------------
// <copyright file="GaugeInputValue.cs" company="syncfusion">
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
    public class GaugeInputValue
    {
        private float addConstant;

        private string dataElementName;

        private DataElementOutputGauge dataElementOutput = DataElementOutputGauge.Output;

        private Formula formula = Formula.None;

        private float maxPercent;

        private float minPercent;

        private float multiplier;

        private string value1;

        public GaugeInputValue()
        {
        }

        public float AddConstant
        {
            get { return addConstant; }

            set { addConstant = value; }
        }

        public string DataElementName
        {
            get { return dataElementName; }

            set { dataElementName = value; }
        }

        public DataElementOutputGauge DataElementOutput
        {
            get { return dataElementOutput; }

            set { dataElementOutput = value; }
        }

        public Formula Formula
        {
            get { return formula; }

            set { formula = value; }
        }

        public float MaxPercent
        {
            get { return maxPercent; }

            set { maxPercent = value; }
        }

        public float MinPercent
        {
            get { return minPercent; }

            set { minPercent = value; }
        }

        public float Multiplier
        {
            get { return multiplier; }

            set { multiplier = value; }
        }

        public string Value
        {
            get { return value1; }

            set { value1 = value; }
        }
    }
}
