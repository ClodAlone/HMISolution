#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Syncfusion.Tools.Silverlight.VisualStudio.Design.Infrastructure;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Media;


namespace Syncfusion.Tools.Silverlight.VisualStudio.Design
{
    internal class RangeSliderInitializer : DefaultInitializer
    {
        public RangeSliderInitializer()
            : base() 
        {
        }

        public override void InitializeDefaults(ModelItem item, EditingContext context) 
        {
            Utils.SparseSetValue(item.Properties["Width"], 400d);
            Utils.SparseSetValue(item.Properties["Height"], 200d);


           
           // ModelItem newautoComplete = ModelFactory.CreateItem(context, AutoCompleteTypes.AutoComplete.TypeId, CreateOptions.InitializeDefaults);
            

        }
    }
}
