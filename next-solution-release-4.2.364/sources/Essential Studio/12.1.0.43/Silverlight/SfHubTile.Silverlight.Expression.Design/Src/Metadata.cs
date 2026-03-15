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
using Microsoft.Windows.Design.Metadata;
using System.ComponentModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Media;
using System.Windows.Controls;
using Microsoft.Windows.Design;
using Syncfusion.Tools.Controls.Notification;

[assembly: ProvideMetadata(typeof(Syncfusion.SfHubTile.Silverlight.Expression.Design.Metadata))]

namespace Syncfusion.SfHubTile.Silverlight.Expression.Design
{
    internal class Metadata : IProvideAttributeTable
    {
        #region IProvideAttributeTable Members

        public AttributeTable AttributeTable
        {
            get
            {

                AttributeTableBuilder builder = new AttributeTableBuilder();
                // tool box filtering
 	            builder.AddCallback(typeof(Syncfusion.Tools.Controls.Notification.SfHubTile), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(Syncfusion.Tools.Controls.Notification.SfMosaicTile), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(Syncfusion.Tools.Controls.Notification.SfPulsingTile), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(Syncfusion.Tools.Controls.Notification.SfSplitMosaicTile), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(HubTileBase), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(true)));
                builder.AddCallback(typeof(HubTileService), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(ImageList), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(MosaicTileContent), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                builder.AddCallback(typeof(HubTileTransitionCollection), b => b.AddCustomAttributes(new ToolboxBrowsableAttribute(false)));
                return builder.CreateTable();

            }
        }

        #endregion
    }
}
