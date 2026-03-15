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
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Diagram;
using Microsoft.Windows.Design.Features;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;

#if SyncfusionFramework4_0 || SyncfusionSLFramework3_0
[assembly: ProvideMetadata(typeof(Syncfusion.Diagram.Silverlight.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.Diagram.Silverlight.VisualStudio.Design
{
#if SyncfusionFramework3_5 && !SyncfusionSLFramework3_0

    public class Metadata : IRegisterMetadata
    {

        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            // tool box filtering
            builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
            //    builder.AddCustomAttributes(typeof(DiagramModel), new ToolboxBrowsableAttribute(false));
            //   builder.AddCustomAttributes(typeof(ConnectorBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LineConnector), new ToolboxBrowsableAttribute(false));
            // builder.AddCustomAttributes(typeof(DiagramViewGrid), new ToolboxBrowsableAttribute(false));
            //     builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DiagramPage), new ToolboxBrowsableAttribute(false));
            //   builder.AddCustomAttributes(typeof(LabelEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Resizer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Node), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(HorizontalRuler), new ToolboxBrowsableAttribute(false));
            // builder.AddCustomAttributes(typeof(TickBar), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(VerticalRuler), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DiagramView), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(PalleteGroupPanel), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(PalleteFilterSelector), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(SymbolPaletteGroup), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(ButtonChecker), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(SymbolItemsControl), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(ResizerThumb), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(Layer), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));

            //  builder.AddCustomAttributes(typeof(DiagramPrintDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ConnectionPort), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Group), new ToolboxBrowsableAttribute(false));
            // builder.AddCustomAttributes(typeof(Gripper), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }

#else

    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                DiagramControlsAttributeTableBuilder builder = new DiagramControlsAttributeTableBuilder();

                // tool box filtering
                //      builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
                //    builder.AddCustomAttributes(typeof(DiagramModel), new ToolboxBrowsableAttribute(false));
                //   builder.AddCustomAttributes(typeof(ConnectorBase), new ToolboxBrowsableAttribute(false));
                // builder.AddCustomAttributes(typeof(DiagramViewGrid), new ToolboxBrowsableAttribute(false));
                //     builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DiagramPage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineConnector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LayoutTransformer), new ToolboxBrowsableAttribute(false));
                //   builder.AddCustomAttributes(typeof(LabelEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Resizer), new ToolboxBrowsableAttribute(false));
                //    builder.AddCustomAttributes(typeof(Node), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(HorizontalRuler), new ToolboxBrowsableAttribute(false));
                // builder.AddCustomAttributes(typeof(TickBar), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(VerticalRuler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Node), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ContextMenuControlItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MenuBase), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(PalleteGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PalleteFilterSelector), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(DiagramViewGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Ruler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalRuler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VerticalRuler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonChecker), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FilterRibbonButton), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(SymbolPaletteGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ButtonChecker), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(SymbolItemsControl), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(ResizerThumb), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(Layer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(WrapPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConnectionPort), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Group), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.Diagram.DiagramControl), new FeatureAttribute(typeof(DiagramControlInitializer)));
                //  builder.AddCustomAttributes(typeof(Gripper), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(DiagramModel), "Nodes", new NewItemTypesAttribute(typeof(Node)));
                //builder.AddCustomAttributes(typeof(DiagramModel), "Connections", new NewItemTypesAttribute(typeof(LineConnector)));
                builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }

    #endregion
    }

#endif

}
