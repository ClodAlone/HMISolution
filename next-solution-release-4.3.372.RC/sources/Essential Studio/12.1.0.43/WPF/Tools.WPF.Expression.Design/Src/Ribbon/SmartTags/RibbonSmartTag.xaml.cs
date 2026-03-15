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

namespace Syncfusion.Tools.WPF.Expression.Design
{
    /// <summary>
    /// Interaction logic for RibbonSmartTag.xaml
    /// </summary>
    public partial class RibbonSmartTag : SmartTagBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonSmartTag"/> class.
        /// </summary>
        public RibbonSmartTag( )
        {
            InitializeComponent( );
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            BindTextBox(NameTextBox, "Name");

            BindSelectorWithEnum(RibbonStateSelector, "RibbonState", typeof(RibbonState));
            BindSelectorWithEnum(HorizontalAlignmentSelector, "HorizontalAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalAlignmentSelector, "VerticalAlignment", typeof(VerticalAlignment));
            BindSelectorWithEnum(HorizontalContentAlignmentSelector, "HorizontalContentAlignment", typeof(HorizontalAlignment));
            BindSelectorWithEnum(VerticalContentAlignmentSelector, "VerticalContentAlignment", typeof(VerticalAlignment));
            
            BindCheckBox(allowDrop, "AllowDrop");
            BindCheckBox(isEnabled, "IsEnabled");
           

        }

        /// <summary>
        /// Adds the child controls.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddChildControls( object sender, RoutedEventArgs e )
        {
            ModelItem item = ModelFactory.CreateItem( this.Context, typeof( RibbonTab ), new object[ 0 ] );

            item.Properties["Caption"].SetValue("New RibbonTab");

            this.ModelItem.Properties[ "Items" ].Collection.Add( item );
        }

        /// <summary>
        /// Adds the item to QAT.
        /// </summary>
        /// <param name="type">The type.</param>
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

        /// <summary>
        /// Adds the ribbon button to QAT.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddRibbonButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( RibbonButton ) );
        }

        /// <summary>
        /// Adds the drop down button to QAT.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddDropDownButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( DropDownButton ) );
        }

        /// <summary>
        /// Adds the split button to QAT.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddSplitButtonToQAT( object sender, RoutedEventArgs e )
        {
            AddItemToQAT( typeof( SplitButton ) );
        } 
    }
}
