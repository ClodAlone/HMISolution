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
using Syncfusion.UI.Xaml.Maps;

[assembly: ProvideMetadata(typeof(Syncfusion.SfMaps.Silverlight.VisualStudio.Design.Metadata))]

namespace Syncfusion.SfMaps.Silverlight.VisualStudio.Design
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
                builder.AddCustomAttributes(typeof(ShapeFileLayer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapColorPalette), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShapeFilePanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(SubShapeFileLayer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(RangeColorMapping), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShapeFillSetting), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(ShapeSetting), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Bubble), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(BubbleMarkerSetting), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(Legend), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(LegendPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapPoint), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapPointPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(CustomDataSymbol), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapAnnotations), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapItem), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapItemSetting), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapItemsPanel), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapLayer), ToolboxBrowsableAttribute.No);
                builder.AddCustomAttributes(typeof(MapShape), ToolboxBrowsableAttribute.No);
                
                

                
                return builder.CreateTable();

            }
        }

        #endregion
    }
}
