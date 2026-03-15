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
using Syncfusion.Windows.Diagram;

namespace Syncfusion.Diagram.Silverlight.VisualStudio.Design
{
    internal class DiagramControlsAttributeTableBuilder : AttributeTableBuilder
    {
        public DiagramControlsAttributeTableBuilder()
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
            
            AddCallback(typeof(DiagramView), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Model", BrowsableAttribute.No);                
            });

            AddCallback(typeof(DiagramPage), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("SelectionList", BrowsableAttribute.No);
            });

            AddCallback(typeof(DiagramView), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("SelectionList", BrowsableAttribute.No);
            });

            AddCallback(typeof(DiagramView), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Content", BrowsableAttribute.No);
            });

            AddCallback(typeof(DiagramView), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("ShowPage", BrowsableAttribute.No);
            });

            AddCallback(typeof(DiagramModel), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Nodes", BrowsableAttribute.No);
            });

            AddCallback(typeof(DiagramModel), delegate(AttributeCallbackBuilder builder)
            {
                builder.AddCustomAttributes("Connections", BrowsableAttribute.No);
            });         

        }
    }
}


