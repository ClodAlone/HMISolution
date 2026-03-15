#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if SyncfusionFramework4_0
using Microsoft.Windows.Design.Metadata;
[assembly: ProvideMetadata(typeof(Syncfusion.SfBarcode.WPF.VisualStudio.Design.Metadata))]
#endif

namespace Syncfusion.SfBarcode.WPF.VisualStudio.Design
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Microsoft.Windows.Design;
    using Microsoft.Windows.Design.Metadata;
    using Microsoft.Windows.Design.PropertyEditing;
    using Syncfusion.UI.Xaml.Controls.Barcode;

#if SyncfusionFramework4_0
    internal class Metadata : IProvideAttributeTable
    {
    #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                //Proceeds the toolbox hidden elements.
                builder.AddCustomAttributes(typeof(Syncfusion.UI.Xaml.Controls.Barcode.SfBarcode), ToolboxBrowsableAttribute.Yes);
                return builder.CreateTable();
            }
        }

    #endregion
    }
#elif SyncfusionFramework3_5
    /// <summary>
    /// Container for any general design-time metadata that we want to initialize.
    /// Designers will look for a type in the design-time assembly that implements IRegisterMetadata.
    /// If found, they will instantiate it and call its Register() method automatically.
    /// </summary>
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            //// Tool box filtering
            builder.AddCustomAttributes(typeof(Syncfusion.UI.Xaml.Controls.Barcode.SfBarcode), new ToolboxBrowsableAttribute(true));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif
}
