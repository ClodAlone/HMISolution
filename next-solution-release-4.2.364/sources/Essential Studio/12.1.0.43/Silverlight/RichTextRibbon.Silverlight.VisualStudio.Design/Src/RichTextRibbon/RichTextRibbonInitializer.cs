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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Services;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design.Infrastructure;

namespace Syncfusion.RichTextRibbon.Silverlight.VisualStudio.Design
{
    internal class RichTextRibbonInitializer :DefaultInitializer
    {
        public static readonly string ImagePath = "/Syncfusion.RichTextRibbon.Silverlight;component/OfficeUI";

        public RichTextRibbonInitializer() :base()
        {

        }
        public override void  InitializeDefaults(ModelItem item, Microsoft.Windows.Design.EditingContext context)
        {          
            base.InitializeDefaults(item);

            if (item.Content != null)
            {
                item.Content.Collection.Clear();
            }
            ModelItem firstRtbItem = GetFirstInitializedRTB(item.Root);

            if (firstRtbItem != null)
            {
                ModelItem elementBinding = ModelFactory.CreateItem(item.Context, typeof(Binding), new object[0]);
                elementBinding.Properties["ElementName"].SetValue(firstRtbItem.Name);
                item.Properties["DataContext"].SetValue(elementBinding);
            }
        }

        internal ModelItem GetFirstInitializedRTB(ModelItem rootItem)
        {
            if (rootItem != null)
            {
                if (rootItem.IsItemOfType(typeof(RichTextBoxAdv)))
                {
                    return rootItem;
                }

                if (rootItem.Content.IsCollection)
                {
                    foreach (ModelItem item in rootItem.Content.Collection)
                    {
                        return GetFirstInitializedRTB(item);
                    }
                }
                else
                {
                    return GetFirstInitializedRTB(rootItem.Content.Value);
                }
            }
            return null;
        }        
    }
}
