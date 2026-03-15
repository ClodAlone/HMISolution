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

    using Microsoft.Windows.Design.Features;
    using Microsoft.Windows.Design.Metadata;
    using Syncfusion.Windows.Gauge;
    using Syncfusion.Windows.Design;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
   
    
// The ProvideMetadata assembly-level attribute indicates to designers
// that this assembly contains a class that provides an attribute table. 
#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Gauge.WPF.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.Gauge.WPF.VisualStudio.Design
{
    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
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
                builder.AddCustomAttributes(typeof(GaugeAdorner), new ToolboxBrowsableAttribute(false));
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

                //Rolling Gauge
                builder.AddCustomAttributes(typeof(UnitPosition), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RollingCharacter), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Direction), new ToolboxBrowsableAttribute(false));
            

                builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(CircularGaugeInitializer)));
                builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(CircularGaugeAdornerProvider)));

                builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PointerCapInitializer)));
                builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PointerCapAdornerProvider)));
                builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(CircularScaleInitializer)));
                builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(CircularScaleAdornerProvider)));
                builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(CircularRangeInitializer)));
                builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(CircularRangeAdornerProvider)));
                builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(DigitalGaugeInitializer)));
                builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(DigitalGaugeAdornerProvider)));


                builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(LinearScaleInitializer)));
                builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(LinearScaleAdornerProvider)));
                builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(StateIndicatorInitializer)));
                builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(StateIndicatorAdornerProvider)));
                builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(LinearMarkerPointerInitializer)));
                builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(LinearMarkerPointerAdornerProvider)));
                builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(LinearGaugeInitializer)));
                builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(LinearGaugeAdornerProvider)));

                
                builder.AddCustomAttributes(typeof(CircularScale), "Ranges", new NewItemTypesAttribute(typeof(CircularRange)));
                builder.AddCustomAttributes(typeof(CircularScale), "Pointers", new NewItemTypesAttribute(typeof(CircularPointer)));
                builder.AddCustomAttributes(typeof(CircularScale), "Pointers", new NewItemTypesAttribute(typeof(CircularPointer)));
                builder.AddCustomAttributes(typeof(CircularScale), "Ticks", new NewItemTypesAttribute(typeof(CircularMarkTick), typeof(CircularLabelTick)));
                builder.AddCustomAttributes(typeof(LinearScale), "Ranges", new NewItemTypesAttribute(typeof(LinearRange)));
                builder.AddCustomAttributes(typeof(LinearScale), "Pointers", new NewItemTypesAttribute(typeof(LinearMarkerPointer), typeof(LinearBarPointer)));
                builder.AddCustomAttributes(typeof(LinearScale), "Ticks", new NewItemTypesAttribute(typeof(LinearMarkTick), typeof(LinearLabelTick)));

                //rolling gauge
                builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(RollingGaugeInitializer)));
                builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(RollingGaugeAdornerProvider)));
                builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

                //primary selection task provider
                builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
                builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

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

            builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(CircularGaugeInitializer)));
            builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(CircularGaugeAdornerProvider)));


            builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PointerCapInitializer)));
            builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PointerCapAdornerProvider)));
            builder.AddCustomAttributes(typeof(PointerCap), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(CircularScaleInitializer)));
            builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(CircularScaleAdornerProvider)));
            builder.AddCustomAttributes(typeof(CircularScale), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            //builder.AddCustomAttributes(typeof(CircularPointer), new FeatureAttribute(typeof(CircularPointerInitializer)));
            //builder.AddCustomAttributes(typeof(CircularPointer), new FeatureAttribute(typeof(CircularPointerAdornerProvider)));

            builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(CircularRangeInitializer)));
            builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(CircularRangeAdornerProvider)));
			builder.AddCustomAttributes(typeof(CircularRange), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(DigitalGaugeInitializer)));
            builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(DigitalGaugeAdornerProvider)));
			

            builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(LinearScaleInitializer)));
            builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(LinearScaleAdornerProvider)));
			builder.AddCustomAttributes(typeof(LinearScale), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(StateIndicatorInitializer)));
            builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(StateIndicatorAdornerProvider)));
			builder.AddCustomAttributes(typeof(StateIndicator), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(LinearMarkerPointerInitializer)));
            builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(LinearMarkerPointerAdornerProvider)));
			builder.AddCustomAttributes(typeof(LinearMarkerPointer), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(LinearGaugeInitializer)));
            builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(LinearGaugeAdornerProvider)));

            builder.AddCustomAttributes(typeof(CircularScale), "Ranges", new NewItemTypesAttribute(typeof(CircularRange)));
            builder.AddCustomAttributes(typeof(CircularScale), "Pointers", new NewItemTypesAttribute(typeof(CircularPointer)));
            builder.AddCustomAttributes(typeof(CircularScale), "Pointers", new NewItemTypesAttribute(typeof(CircularPointer)));
            builder.AddCustomAttributes(typeof(CircularScale), "Ticks", new NewItemTypesAttribute(typeof(CircularMarkTick), typeof(CircularLabelTick)));
            builder.AddCustomAttributes(typeof(LinearScale), "Ranges", new NewItemTypesAttribute(typeof(LinearRange)));
            builder.AddCustomAttributes(typeof(LinearScale), "Pointers", new NewItemTypesAttribute(typeof(LinearMarkerPointer), typeof(LinearBarPointer)));
            builder.AddCustomAttributes(typeof(LinearScale), "Ticks", new NewItemTypesAttribute(typeof(LinearMarkTick), typeof(LinearLabelTick)));

            builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(RollingGaugeInitializer)));
            builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(RollingGaugeAdornerProvider)));
            builder.AddCustomAttributes(typeof(RollingGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));

            builder.AddCustomAttributes(typeof(CircularGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(LinearGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            builder.AddCustomAttributes(typeof(DigitalGauge), new FeatureAttribute(typeof(PrimarySelectionTaskProviderBase)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif

    internal class GetChildCount:SmartTagBase
    {
        internal string GetSuffix(string childname, string type,ModelItem item)
        {
            string suffix = "1";

            if (item.Properties[type].Collection.Count == 0)
                return suffix;
            
            string name = item.Properties[type].Collection[item.Properties[type].Collection.Count - 1].Name;

            suffix = ((int.Parse(name.Substring(name.IndexOf(childname) + childname.Length))) + 1).ToString();
            
            return suffix;
        }
    }
}