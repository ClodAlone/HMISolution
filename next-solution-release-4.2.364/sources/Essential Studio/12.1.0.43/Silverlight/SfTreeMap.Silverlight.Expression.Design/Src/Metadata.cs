#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Media;
using Microsoft.Windows.Design;
using Syncfusion.UI.Xaml.TreeMap;

[assembly: ProvideMetadata(typeof(Syncfusion.SfTreeMap.Silverlight.Expression.Design.Metadata))]

namespace Syncfusion.SfTreeMap.Silverlight.Expression.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering
                builder.AddCustomAttributes(typeof(ColorMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeBrushColorMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UniColorMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PaletteColorMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DesaturationColorMapping), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeBrush), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapItem), new ToolboxBrowsableAttribute(false));
				builder.AddCustomAttributes(typeof(TreeMapLegend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapLegendItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapLegendPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapLevel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapFlatLevel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapHierarchicalLevel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapSubItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TreeMapLeafNode), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeBrush), new ToolboxBrowsableAttribute(false));
                
                return builder.CreateTable();

            }
        }

        #endregion
    }
}
