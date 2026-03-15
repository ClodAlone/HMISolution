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
using Syncfusion.Windows.Gauge;
using Microsoft.Windows.Design;

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.Gauge.Silverlight.Expression.Design.RegisterGaugeControlsMetadata))]
#endif

namespace Syncfusion.Gauge.Silverlight.Expression.Design
{
#if SyncfusionFramework3_5

    internal class Metadata : IRegisterMetadata
    {
    #region IRegisterMetadata Members

        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCallback(typeof(GaugeBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CircularPointer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CircularRange), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GaugeElement), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LocalizableGaugeElement), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScaleBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CircularScale), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(PointerCap), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(ScalesLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GaugeElementPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GaugeBorder), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GaugeImage), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(GaugeLabel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LabelTick), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LabelTickSet), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(MarkTickSet), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(StateIndicator), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(StateRange), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LinearBarPointer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LinearMarkerPointer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LinearRange), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LinearScale), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(LinearScaleLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(DigitalGaugeLayoutPanel), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(CircularKnob), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            
            builder.AddCallback(typeof(PointerBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));  
            builder.AddCallback(typeof(RangeBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));  
            builder.AddCallback(typeof(TickSetBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));  
            builder.AddCallback(typeof(LinearPointer), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));  

            builder.AddCallback(typeof(RollingCharacter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

            builder.AddCallback(typeof(CircularGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(LinearGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(DigitalGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            builder.AddCallback(typeof(RollingGauge), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        

    #endregion
    }


#elif SyncfusionFramework4_0

    internal class RegisterGaugeControlsMetadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                GaugeControlsAttributeTableBuilder builder = new GaugeControlsAttributeTableBuilder();

                builder.AddCustomAttributes(typeof(GaugeBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LocalizableGaugeElement), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScaleBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularScale), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(PointerCap), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(ScalesLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeElementPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeBorder), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeImage), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(GaugeLabel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LabelTick), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LabelTickSet), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(MarkTickSet), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StateIndicator), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(StateRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearBarPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearMarkerPointer), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearRange), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearScale), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearScaleLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(DigitalGaugeLayoutPanel), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(CircularKnob), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RollingCharacter), new ToolboxBrowsableAttribute(false));

                builder.AddCustomAttributes(typeof(PointerBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(RangeBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(TickSetBase), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(LinearPointer), new ToolboxBrowsableAttribute(false));


                builder.AddCustomAttributes(typeof(CircularGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(LinearGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(DigitalGauge), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(RollingGauge), new ToolboxBrowsableAttribute(true));

                return builder.CreateTable();
            }
        }

        #endregion
    }

#endif
}
