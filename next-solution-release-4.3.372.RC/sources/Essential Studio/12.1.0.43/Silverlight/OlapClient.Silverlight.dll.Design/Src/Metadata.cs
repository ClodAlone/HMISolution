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
using Microsoft.Windows.Design;
using Syncfusion.Silverlight.Client.Olap;

namespace Syncfusion.OlapClient.Silverlight.dll.Design
{

#if SyncfusionFramework4_0 || SyncfusionSLFramework3_5

    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCallback(typeof(Syncfusion.Silverlight.Client.Olap.OlapClient), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));

                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.AxisElementBuilder ), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CDTreeView), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CDTreeViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CubeDimensionBrowser), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MeasureEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditorTreeView), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditorTreeViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.OlapToolBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.OlapToolBarButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.ReportNameGetter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.SplitButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.DownButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.ConnectionDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MdxDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CalcMemberEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
				builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.VirtualKpiEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));

                return builder.CreateTable();
            }
        }

        #endregion
    }
#else

    public class Metadata : IRegisterMetadata
    {
        #region IRegisterMetadata Members

        public void Register()
        {
            AttributeTableBuilder builder = new AttributeTableBuilder();

            builder.AddCallback(typeof(Syncfusion.Silverlight.Client.Olap.OlapClient), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));

            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.AxisElementBuilder ), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CDTreeView), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CDTreeViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.CubeDimensionBrowser), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MeasureEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditorTreeView), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MemberEditorTreeViewItem), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.OlapToolBar), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.OlapToolBarButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.ReportNameGetter), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.SplitButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.DownButton), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.ConnectionDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.MdxDialog), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            builder.AddCallback(typeof(Syncfusion.Silverlight.Tools.Olap.VirtualKpiEditor), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
            MetadataStore.AddAttributeTable(builder.CreateTable());
        }

        #endregion
    }

#endif
}
