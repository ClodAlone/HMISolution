#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using Microsoft.Windows.Design;
using System.Windows;
using Syncfusion.Windows.Gauge;

namespace Syncfusion.Gauge.Silverlight.Expression.Design
{
    internal class GaugeControlsAttributeTableBuilder : AttributeTableBuilder
    {
        public GaugeControlsAttributeTableBuilder()
            : base()
        {
            AddAllGaugeAttributes();
        }

        private void AddAllGaugeAttributes()
        {
            AddGaugeControlAttributes();

        }

        private void AddGaugeControlAttributes()
        {
            AddCallback(typeof(CircularGauge), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(CircularGaugeInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(LinearGauge), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(LinearGaugeInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(DigitalGauge), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(DigitalGaugeInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(RollingGauge), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(RollingGaugeInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

        }
    }
}


