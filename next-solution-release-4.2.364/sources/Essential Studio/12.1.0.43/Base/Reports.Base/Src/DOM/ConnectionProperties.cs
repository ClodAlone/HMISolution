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
    public class ConnectionProperties
    {
        public string DataProvider { get; set; }
        public string ConnectString { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Prompt { get; set; }

        [XmlIgnore]
        public string UserName { get; set; }

        [XmlIgnore]
        public string PassWord { get; set; }

        public ConnectionProperties()
        {

        }

        public ConnectionProperties(ConnectionProperties connectionProperties)
        {
            this.DataProvider = connectionProperties.DataProvider;
            this.ConnectString = connectionProperties.ConnectString;
            this.IntegratedSecurity = connectionProperties.IntegratedSecurity;
            this.Prompt = connectionProperties.Prompt;
            this.UserName = connectionProperties.UserName;
            this.PassWord = connectionProperties.PassWord;
        }

        public bool ShouldSerializeIntegratedSecurity()
        {
            return this.IntegratedSecurity != false;
        }

        public void ResetIntegratedSecurity()
        {
            this.IntegratedSecurity = false;
        }
    }
}
