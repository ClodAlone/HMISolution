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
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Diagram;
using Microsoft.Windows.Design.Features;
using Syncfusion.Windows.Design;
    

// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
#if !SyncfusionFramework3_5
[assembly: ProvideMetadata(typeof(Syncfusion.Diagram.WPF.dll.Design.Metadata))]
#endif
namespace Syncfusion.Diagram.WPF.dll.Design
{

    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
#if !SyncfusionFramework3_5
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                // Called by Cider to register any design-time metadata
                //public void Register() {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
              
                builder.AddCustomAttributes(typeof(SnapSettings), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DiagramModel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConnectorBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LineConnector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramViewGrid), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DiagramPage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DragProvider), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LabelEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NodeConnectorAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(NodeSelectionAdorner), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(RotateOriginPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Rotator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RotateThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Resizer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Node), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HorizontalRuler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TickBar), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(VerticalRuler), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(PalleteGroupPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PalleteFilterSelector), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FilterRibbonButton), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPaletteGroup), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ButtonChecker), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(SymbolItemsControl), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ResizerThumb), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Layer), new ToolboxBrowsableAttribute(false));
                //builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DiagramPrintDialog), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ConnectionPort), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Group), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Gripper), new ToolboxBrowsableAttribute(false));

                //builder.AddCustomAttributes(typeof(DiagramControl), new Microsoft.Windows.Design.Features.FeatureAttribute(typeof(Syncfusion.Diagram.WPF.dll.Design.DiagramControlInitializer)));
                //builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(DiagramControlAdornerProvider)));

                builder.AddCustomAttributes(typeof(HorizontalRuler), new FeatureAttribute(typeof(HorizontalRulerAdornerProvider)));
                builder.AddCustomAttributes(typeof(HorizontalRuler), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(VerticalRuler), new FeatureAttribute(typeof(VerticalRulerAdornerProvider)));
                builder.AddCustomAttributes(typeof(VerticalRuler), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(DiagramControlAdornerProvider)));
                builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DiagramPage), new FeatureAttribute(typeof(DiagramPageAdornerProvider)));
                builder.AddCustomAttributes(typeof(DiagramPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DiagramView), new FeatureAttribute(typeof(DiagramViewAdornerProvider)));
                builder.AddCustomAttributes(typeof(DiagramView), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
             
                builder.AddCustomAttributes(typeof(SnapSettings), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DiagramModel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ConnectorBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LineConnector), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DiagramViewGrid), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DiagramControl), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DiagramPage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DragProvider), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LabelEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NodeConnectorAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(NodeSelectionAdorner), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(RotateOriginPointer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Rotator), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RotateThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Resizer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Node), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HorizontalRuler), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(TickBar), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(VerticalRuler), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DiagramView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPalette), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(PalleteGroupPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PalleteFilterSelector), new ToolboxBrowsableAttribute(false));
    builder.AddCustomAttributes(typeof(FilterRibbonButton), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPaletteGroup), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(SymbolPaletteItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ButtonChecker), new ToolboxBrowsableAttribute(false));
            //builder.AddCustomAttributes(typeof(SymbolItemsControl), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ResizerThumb), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Layer), new ToolboxBrowsableAttribute(false));

            builder.AddCustomAttributes(typeof(DiagramPrintDialog), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ConnectionPort), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Group), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Gripper), new ToolboxBrowsableAttribute(false));

            //builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(DiagramControlInitializer)));
            builder.AddCustomAttributes(typeof(HorizontalRuler), new FeatureAttribute(typeof(HorizontalRulerAdornerProvider)));
            builder.AddCustomAttributes(typeof(HorizontalRuler), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(VerticalRuler), new FeatureAttribute(typeof(VerticalRulerAdornerProvider)));
            builder.AddCustomAttributes(typeof(VerticalRuler), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(DiagramControlAdornerProvider)));
            builder.AddCustomAttributes(typeof(DiagramControl), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DiagramPage), new FeatureAttribute(typeof(DiagramPageAdornerProvider)));
            builder.AddCustomAttributes(typeof(DiagramPage), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DiagramView), new FeatureAttribute(typeof(DiagramViewAdornerProvider)));
            builder.AddCustomAttributes(typeof(DiagramView), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(FourQuadrantPanel), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
