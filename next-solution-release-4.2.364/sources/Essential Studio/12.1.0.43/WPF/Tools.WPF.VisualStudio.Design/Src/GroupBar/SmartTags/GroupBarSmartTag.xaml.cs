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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Tools;
using System.Diagnostics;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for AppMenuSmartTag.xaml
    /// </summary>
    public partial class GroupBarSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the GroupBarSmartTag class.
        /// </summary>
        public GroupBarSmartTag()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method is called when the GroupBarSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(NameTextBox, "Name");

            BindSelectorWithEnum(OrientationSelector, "Orientation", typeof(Orientation));
            BindSelectorWithEnum(VisualModeSelector, "VisualMode", typeof(VisualMode));
            BindSelectorWithEnum(GroupBarTextAlignment, "TextAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(AnimationMode, "AnimationType", typeof(AnimationType));
            BindSelectorWithEnum(PopupResizeDirection, "PopupResizeDirection", typeof(PopupResizeDirection));
            


        

            BindCheckBox(AllowCollapseCheckBox, "AllowCollapse");
            BindCheckBox(ShowContextMenu, "IsEnabledContextMenu");
            BindCheckBox(ShowGripper, "ShowGripper");
           
        }

        private void AddChildControls(object sender, RoutedEventArgs e)
        {
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(GroupBarItem), new object[0]);
            //int i = 0;
            //string str = string.Empty;
            int count = this.ModelItem.Properties["Items"].Collection.Count;
            //foreach (ModelItem mod in this.ModelItem.Properties["Items"].Collection)
            //{
            //    if (mod.Name.Length > 11)
            //   {
            //        try
            //        {
            //            if (i < Convert.ToInt32(mod.Name.Substring(12)))
            //            {
            //                i = Convert.ToInt32(mod.Name.Substring(12));
            //            }
            //        }
            //        catch { }
            //    }
            //}
            //i++;
            this.ModelItem.Properties["Items"].Collection.Add(item);

            this.ModelItem.Properties["Items"].Collection[count].Properties["Header"].SetValue("New GroupBarItem");

            //this.ModelItem.Properties["Items"].Collection[count].Properties["Name"].SetValue("GroupBarItem" + i.ToString()); 
        }

    }

}        

       


        
        
   

