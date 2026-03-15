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
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Windows.Design.Metadata;
    using Syncfusion.Windows.Gauge;
    using Microsoft.Windows.Design;
    
    // The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Gauge.WPF.dll.Design.Metadata))]
#endif
namespace Syncfusion.Gauge.WPF.dll.Design
{
    // Container for any general design-time metadata to initialize.
    // Designers look for a type in the design-time assembly that 
    // implements IProvideAttributeTable. If found, designers instantiate 
    // this class and access its AttributeTable property automatically.    
    #if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
        // Accessed by the designer to register any design-time metadata.
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                // tool box filtering
                builder.AddCustomAttributes(typeof(CircularGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(LinearGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DigitalGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(RollingGauge), new ToolboxBrowsableAttribute(true));

                builder.AddCustomAttributes(typeof(GaugeBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularGaugeAdorner), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularCustomLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularLabelTick), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularMarkTick), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularScale), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeFrameType), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeImage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeImageResizeMode), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LocalizableGaugeElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PointerCap), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(HalfCircleBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScalesLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StateRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearScaleLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearMarkerPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearScale), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StateRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeCustomLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearBarPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearMarkTick), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DigitalGaugeLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(UnitPosition), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RollingCharacter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Direction), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#elif SyncfusionFramework3_5
    internal class Metadata : IRegisterMetadata {

        // Called by Cider to register any design-time metadata
        public void Register() {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            // tool box filtering
            builder.AddCustomAttributes(typeof(CircularGauge), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(LinearGauge), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(DigitalGauge), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(RollingGauge), new ToolboxBrowsableAttribute(true));

            builder.AddCustomAttributes(typeof(GaugeBase), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularGaugeAdorner), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularCustomLabel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularLabelTick), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularMarkTick), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularPointer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularRange), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(CircularScale), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GaugeElement), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GaugeFrameType), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GaugeImage), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GaugeImageResizeMode), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LocalizableGaugeElement), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(PointerCap), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(HalfCircleBorder), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(ScalesLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(StateRange), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LinearScaleLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LinearMarkerPointer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LinearScale), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(StateRange), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(GaugeCustomLabel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LinearBarPointer), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(LinearMarkTick), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(DigitalGaugeLayoutPanel), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(UnitPosition), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(RollingCharacter), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Direction), new ToolboxBrowsableAttribute(false));

            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
 