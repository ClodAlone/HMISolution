#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.SfBarcode.Silverlight.Expression.Design
{
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Syncfusion.UI.Xaml.Controls.Barcode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ProvideMetadata(typeof(Syncfusion.SfBarcode.Silverlight.Expression.Design.Metadata))]


    public class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering               
                builder.AddCustomAttributes(typeof(Syncfusion.UI.Xaml.Controls.Barcode.SfBarcode), new ToolboxBrowsableAttribute(false));
                

                return builder.CreateTable();

            }
        }

        #endregion
    }
}
