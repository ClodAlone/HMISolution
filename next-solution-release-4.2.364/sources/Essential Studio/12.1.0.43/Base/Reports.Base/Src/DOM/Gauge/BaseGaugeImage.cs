//-------------------------------------------------------------------------------------------------
// <copyright file="BaseGaugeImage.cs" company="syncfusion">
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
    public class BaseGaugeImage
    {
        private string mIMEType;

        private Source source;

        private string transparentColor;

        private object value1;

        public BaseGaugeImage()
        {
        }

        public string MIMEType
        {
            get { return mIMEType; }
            set { mIMEType = value; }
        }

        public Source Source
        {
            get { return source; }
            set { source = value; }
        }

        public string TransparentColor
        {
            get { return transparentColor; }
            set { transparentColor = value; }
        }

         public object Value
        {
            get 
            { 
                return value1; 
            }

            set 
            { 
                value1 = value; 
            }
        }
    }
}
