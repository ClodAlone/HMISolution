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
using Syncfusion.Windows.Tools.Controls;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Model;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Design;

namespace Syncfusion.Tools.WPF.VisualStudio.Design
{
    /// <summary>
    /// Interaction logic for RibbonSmartTag.xaml
    /// </summary>
    public partial class RibbonSmartTag : SmartTagBase
    {
        static int count;
        /// <summary>
        /// Initializes a new instance of the RibbonSmartTag class.
        /// </summary>
        public RibbonSmartTag( )
        {
            InitializeComponent( );
        }
        /// <summary>
        /// This method is called when the RibbonSmartTag Item template is initialized.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(NameTextBox, "Name");

            BindSelectorWithEnum(RibbonStateSelector, "RibbonState", typeof(RibbonState));
            BindSelectorWithEnum(HorizontalAlignmentSelector, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalAlignmentSelector, "VerticalAlignment", typeof(VerticalAlignment));           
            
            BindCheckBox(allowDrop, "AllowDrop");
            BindCheckBox(isEnabled, "IsEnabled");

            count = 2;
        }

        private void AddChildControls( object sender, RoutedEventArgs e )
        {
            count++;
            ModelItem item = ModelFactory.CreateItem(this.Context, typeof(RibbonTab), new object[0]);
          
            int cnt = this.ModelItem.Properties["Items"].Collection.Count;
           
            this.ModelItem.Properties["Items"].Collection.Add(item);

            this.ModelItem.Properties["Items"].Collection[cnt].Properties["Caption"].SetValue("New RibbonTab" + count.ToString());

            //this.ModelItem.Properties["Items"].Collection[cnt].Properties["Name"].SetValue("RibbonTab" + count.ToString()); 
        
        }
        
        private void AddItemToQAT( Type type )
        {
            ModelItem qatModelItem = this.ModelItem.Properties[ "QuickAccessToolBar" ].Value;

            if( qatModelItem != null )
            {
                ModelItem item = ModelFactory.CreateItem( this.Context, type, new object[ 0 ] );
                if( item.View is ICollapsable )
                {
                    item.Properties[ "SizeForm" ].SetValue( SizeForm.ExtraSmall );
                }
                qatModelItem.Properties[ "Items" ].Collection.Add( item );
            }
        }

        private void AddRibbonButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( RibbonButton ) );
        }

        private void AddDropDownButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( DropDownButton ) );
        }

        private void AddSplitButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( SplitButton ) );
        } 
    }
}
