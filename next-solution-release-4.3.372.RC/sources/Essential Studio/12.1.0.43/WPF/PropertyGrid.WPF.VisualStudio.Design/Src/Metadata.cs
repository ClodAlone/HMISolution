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
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.PropertyGrid;
using Microsoft.Windows.Design;

#if SyncfusionFramework4_0
[assembly: ProvideMetadata(typeof(Syncfusion.PropertyGrid.WPF.VisualStudio.Design.Metadata))]
#endif
namespace Syncfusion.PropertyGrid.WPF.VisualStudio.Design
{
#if SyncfusionFramework4_0 
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new FeatureAttribute(typeof(PropertyGridInitializer)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#elif SyncfusionFramework4_5
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new ToolboxBrowsableAttribute(true));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyView), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new FeatureAttribute(typeof(PropertyGridInitializer)));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
                builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
                return builder.CreateTable();
            }
        }
    }
#else
    internal class Metadata : IRegisterMetadata
    {
        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new ToolboxBrowsableAttribute(true));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyView), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyCatagoryViewItem), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.PropertyGrid), new FeatureAttribute(typeof(PropertyGridInitializer)));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.CollectionEditor), new ToolboxBrowsableAttribute(false));
            builder.AddCustomAttributes(typeof(Syncfusion.Windows.PropertyGrid.ItemsSourceControl), new ToolboxBrowsableAttribute(false));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }
    }
#endif

    internal class PropertyGridInitializer : DefaultInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorEditorInitializer"/> class.
        /// </summary>
        public PropertyGridInitializer()
        {
        }

        /// <summary>
        /// Initializes default values for the specified item.
        /// </summary>
        /// <param name="item">The item to initialize. This should not be null.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="item"/> is null.
        /// </exception>
        public override void InitializeDefaults(ModelItem item)
        {
            using (ModelEditingScope scope = item.BeginEdit())
            {
                scope.Complete();
            }
        }
    }
}
