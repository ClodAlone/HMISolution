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
using System.Windows.Controls;
using Microsoft.Windows.Design;
using Syncfusion.UI.Xaml.TreeMap;

[assembly: ProvideMetadata(typeof(Syncfusion.SfTreeMap.WPF.Expression.Design.Metadata))]

namespace Syncfusion.SfTreeMap.WPF.Expression.Design
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
                builder.AddCustomAttributes(typeof(ColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeBrushColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(UniColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(PaletteColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(DesaturationColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeBrush), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapItem), ToolboxBrowsableAttribute.No);
				builder.AddCustomAttributes(typeof(TreeMapLegend), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapLegendItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapLegendPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapLevel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapFlatLevel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapHierarchicalLevel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapSubItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(TreeMapLeafNode), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeBrush), ToolboxBrowsableAttribute.No);

                return builder.CreateTable();
            }
        }

        #endregion
    }
}
