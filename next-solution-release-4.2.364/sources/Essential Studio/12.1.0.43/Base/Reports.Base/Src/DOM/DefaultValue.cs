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
using System.Xml.Serialization;

namespace Syncfusion.RDL.DOM
{
    public class DefaultValue
    {
        public DataSetReference DataSetReference { get; set; }
        [XmlArray("Values"), XmlArrayItem("Value")]
        public Values Values { get; set; }

        public DefaultValue()
        {
        }

        public DefaultValue(DefaultValue defaultValues)
        {
            if (defaultValues != null)
            {
                if (defaultValues.DataSetReference != null)
                    this.DataSetReference = new DataSetReference(defaultValues.DataSetReference);

                if (defaultValues.Values != null)
                {
                    this.Values = new Values();
                    this.Values.AddRange(defaultValues.Values.Clone());
                }
            }
        }

        public bool ShouldSerializeValues()
        {
            return this.Values != null && this.Values.Count > 0;
        }

        public void ResetValues()
        {
            this.Values = new Values();
        }
    }

    public class Values : List<string>
    {
    }
}
