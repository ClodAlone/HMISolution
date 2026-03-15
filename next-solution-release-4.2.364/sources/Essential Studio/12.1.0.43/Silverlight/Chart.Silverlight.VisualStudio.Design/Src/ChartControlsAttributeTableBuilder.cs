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
using Syncfusion.Chart.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Chart.Silverlight.VisualStudio.Design.Chart;
using Microsoft.Windows.Design;
using Syncfusion.Windows.Chart;

namespace Syncfusion.Chart.Silverlight.VisualStudio.Design 
{
    internal class ChartControlsAttributeTableBuilder : AttributeTableBuilder
    {
        public ChartControlsAttributeTableBuilder()
            : base()
        {
            AddChartControlAttributes();
            AddHeadMapControlAttributes();
            AddSparkLineControlAttributes();
        }

        private void AddChartControlAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.Chart.Chart), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(ChartControlInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });
        }

        private void AddHeadMapControlAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.Chart.HeatMapControl), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(HeatMapInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });
        }
        private void AddSparkLineControlAttributes()
        {
            AddCallback(typeof(Syncfusion.Windows.Chart.SparkLine), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(SparkLineInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });
        }
    }
}


