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
using Syncfusion.Windows.Tools.Controls;

namespace Syncfusion.Tools.Silverlight.Expression.Design
{
    internal class ToolsControlsAttributeTableBuilder : AttributeTableBuilder
    {
        public ToolsControlsAttributeTableBuilder()
            : base()
        {
            AddAllToolsAttributes();
        }

        private void AddAllToolsAttributes()
        {
            AddToolsControlAttributes();

        }

        private void AddToolsControlAttributes()
        {
            AddCallback(typeof(AutoComplete), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(AutoCompleteInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(string))
                    );
            });

            //AddCallback(typeof(BrushEdit), delegate(AttributeCallbackBuilder builder)
            //{
            //    builder.AddCustomAttributes(
            //          new FeatureAttribute(typeof(BrushEditInitializer))
            //        , new ComplexBindingPropertiesAttribute("", "")
            //        , new LookupBindingPropertiesAttribute("", "", "", "")
            //        );
            //});

            //AddCallback(typeof(BrushSelector), delegate(AttributeCallbackBuilder builder)
            //{
            //    builder.AddCustomAttributes(
            //          new FeatureAttribute(typeof(BrushSelectorInitializer))
            //        , new ComplexBindingPropertiesAttribute("", "")
            //        , new LookupBindingPropertiesAttribute("", "", "", "")
            //        );
            //});

            AddCallback(typeof(CalendarControl), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(CalendarControlInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(CheckedListBox), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(CheckedListBoxInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(CheckedListBoxItem))
                    );
            });

            //AddCallback(typeof(ColorPickerPalette), delegate(AttributeCallbackBuilder builder)
            //{
            //    builder.AddCustomAttributes(
            //          new FeatureAttribute(typeof(ColorPickerPaletteInitializer))
            //        , new ComplexBindingPropertiesAttribute("", "")
            //        , new LookupBindingPropertiesAttribute("", "", "", "")
            //        );
            //});

            //AddCallback(typeof(DateTimeEdit), delegate(AttributeCallbackBuilder builder)
            //{
            //    builder.AddCustomAttributes(
            //          new FeatureAttribute(typeof(DateTimeEditControlInitializer))
            //        , new ComplexBindingPropertiesAttribute("", "")
            //        , new LookupBindingPropertiesAttribute("", "", "", "")
            //        );
            //});

            AddCallback(typeof(DomainUpDown), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(DomainUpDownInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(FileUploadControl), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(FileUploadControlInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(GroupBar), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(GroupBarInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(GroupBarItem))
                    );
            });

            AddCallback(typeof(HtmlHost), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(HtmlHostControlInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(NumericUpDown), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(NumericUpDownInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(RangeSlider), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(RangeSliderInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    );
            });

            AddCallback(typeof(TaskBar), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(TaskBarInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(TaskBarItem))
                    );
            });

            AddCallback(typeof(TreeViewAdv), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(TreeViewAdvInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(TreeViewItemAdv))
                    );
            });

            AddCallback(typeof(TabControlAdv), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes(
                      new FeatureAttribute(typeof(TabControlAdvInitializer))
                    , new ComplexBindingPropertiesAttribute("", "")
                    , new LookupBindingPropertiesAttribute("", "", "", "")
                    , new NewItemTypesAttribute(typeof(TabItemAdv))
                    );
            });

            AddCallback(typeof(CalendarControl), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Culture", BrowsableAttribute.No);
                builder.AddCustomAttributes(
                    new FeatureAttribute(typeof(CalendarControlInitializer))
                  );
            });

            //AddCallback(typeof(DateTimeEdit), delegate(AttributeCallbackBuilder builder)
            //{
            //    builder.AddCustomAttributes("Culture", BrowsableAttribute.No);
            //    builder.AddCustomAttributes(
            //        new FeatureAttribute(typeof(DateTimeEditControlInitializer))
            //      );
            //});


        }
    }
}


