#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using System;
using Syncfusion.UI.Xaml.BulletGraph;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: ProvideMetadata(typeof(Syncfusion.SfBulletGraph.Silverlight.VisualStudio.Design.Metadata))]
namespace Syncfusion.SfBulletGraph.Silverlight.VisualStudio.Design
{
    public class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering
                builder.AddCustomAttributes(typeof(BulletGraphLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BulletGraphTick), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BulletGraphUniformPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(QualitativeRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BulletGraphLabelCollection), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(BulletGraphTickCollection), new ToolboxBrowsableAttribute(false));


                return builder.CreateTable();

            }
        }

        #endregion
    }
}
