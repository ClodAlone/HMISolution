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
using System.Xml;

namespace Syncfusion.RDL.DOM
{
    public class DataSetReference
    {
        public string DataSetName { get; set; }
        public string ValueField { get; set; }
        public string LabelField { get; set; }

        public DataSetReference()
        {

        }

        public DataSetReference(DataSetReference dataSetReference)
        {
            if (dataSetReference != null)
            {
                this.DataSetName = dataSetReference.DataSetName;
                this.ValueField = dataSetReference.ValueField;
                this.LabelField = dataSetReference.LabelField;
            }
        }
    }
}
