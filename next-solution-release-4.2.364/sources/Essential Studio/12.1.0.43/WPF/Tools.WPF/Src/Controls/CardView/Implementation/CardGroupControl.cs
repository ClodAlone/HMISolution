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
using System.Windows.Controls;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Windows.Data;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    public class CardGroupControl : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item)
         {
            type = item.GetType();
            return item is CardViewItem;
        }

        public CardGroupControl()
        {
            mgeneric = new ResourceDictionary();
            mgeneric.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
        }

        private Type type;

        private ResourceDictionary mgeneric;

        protected override DependencyObject GetContainerForItemOverride()
        {
            if (type.Name.ToString() == "CollectionViewGroupInternal")
            {
                string s = SkinStorage.GetVisualStyle(this).ToString();
                ResourceDictionary rs = new ResourceDictionary();
                if (s == "Default")
                    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml", UriKind.RelativeOrAbsolute);
                else
                    rs.Source = new Uri("/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/" + s + "Style.xaml", UriKind.RelativeOrAbsolute);
               
                ControlTemplate temp = rs["GroupStyle1"] as ControlTemplate;
                Expander exp = temp.LoadContent() as Expander;               
                ItemContainerStyle = null;
                return exp;
            }
            else
            {
                return new CardViewItem();                
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            //(element as CardViewItem).cardView = VisualUtils.FindAncestor(this, typeof(CardView)) as CardView;
            if (element != null && element as CardViewItem != null)
            {
                (element as CardViewItem).cardView = VisualUtils.FindAncestor(this, typeof(CardView)) as CardView;                
                (element as CardViewItem).cardGroup = this as CardGroupControl;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

      
    }

}
