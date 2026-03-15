#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Microsoft.Windows.Design.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.Windows.Controls.Map;
using Microsoft.Windows.Design;
#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0 || SyncfusionFramework4_5
using Microsoft.Windows.Design.Features;

[assembly: ProvideMetadata(typeof(Syncfusion.Maps.Silverlight.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.Maps.Silverlight.VisualStudio.Design
{
#if SyncfusionFramework4_0 || SyncfusionSLFramework4_0 || SyncfusionFramework4_5
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(MapControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(NavigationControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ShapeFileLayer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapLayer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Layers), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ShapeFileCanvas), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ShapeFilePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapSymbols), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapShapes), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPalettePanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Legend), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LegendPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapElementPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MapPath), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Tile), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ImageryPanel), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(MapControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(NavigationControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ShapeFileLayer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapLayer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Layers), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ShapeFileCanvas), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ShapeFilePanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapLabel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapSymbols), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapShapes), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPalettePanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Legend), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LegendPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapElementPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(MapPath), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Tile), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ImageryPanel), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
