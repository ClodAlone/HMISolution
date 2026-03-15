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
    public class ValidValues
    {
        public DataSetReference DataSetReference { get; set; }
        public ParameterValues ParameterValues { get; set; }

        public ValidValues()
        {
        }

        public ValidValues(ValidValues validValues)
        {
            if (validValues != null)
            {
                if (validValues.DataSetReference != null)
                    this.DataSetReference = new DataSetReference(validValues.DataSetReference);

                if (validValues!=null && validValues.ParameterValues != null)
                {
                    this.ParameterValues = new ParameterValues();
                    this.ParameterValues.AddRange(validValues.ParameterValues.Clone());
                }
            }
        }

        public bool ShouldSerializeParameterValues()
        {
            return this.ParameterValues != null && this.ParameterValues.Count > 0;
        }

        public void ResetParameterValues()
        {
            this.ParameterValues = new ParameterValues();
        }
    }
}
