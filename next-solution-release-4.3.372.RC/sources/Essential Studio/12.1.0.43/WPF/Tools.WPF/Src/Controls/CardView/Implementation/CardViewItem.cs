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
using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Data;
using System.Globalization;

namespace Syncfusion.Windows.Tools.Controls
{
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
          Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2007SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010BlueStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010BlackStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Office2010SilverStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
  Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/BlendStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.VS2010,
 Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/VS2010Style.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
 Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/Generic.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/MetroStyle.xaml")]
    [SkinType(SkinVisualStyle = Skin.Transparent,
Type = typeof(CardViewItem), XamlResource = "/Syncfusion.Tools.WPF;component/Controls/CardView/Themes/TransparentStyle.xaml")]  
    public class CardViewItem : HeaderedContentControl 
    {
      
        internal CardView cardView;

        internal CardGroupControl cardGroup;

        private ContentPresenter _header;

        internal ContentPresenter _content;

        public CardViewItem()
        {
           
        }

        public override void OnApplyTemplate()
        {
            //cardView = VisualUtils.FindAncestor(this, typeof(CardView)) as CardView;
            _header = GetTemplateChild("PART_HeaderPresenter") as ContentPresenter;
            _content = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
            base.OnApplyTemplate();
        }

        static CardViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardViewItem), new FrameworkPropertyMetadata(typeof(CardViewItem)));
        }

        public bool IsInEditMode
        {
            get { return (bool)GetValue(IsInEditModeProperty); }
            internal set { SetValue(IsInEditModeProperty, value); }
        }

         //Using a DependencyProperty as the backing store for IsInEditMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInEditModeProperty =
            DependencyProperty.Register("IsInEditMode", typeof(bool), typeof(CardViewItem), new PropertyMetadata(null));

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {            
            if (cardView != null && !cardView.IsGrouping)
            {
                IsSelected = true;
                for (int i = 0; i < cardView.Items.Count; i++)
                {
                    CardViewItem item = cardView.ItemContainerGenerator.ContainerFromIndex(i) as CardViewItem;
                    if (item != null && item != this)
                    {
                        item.IsSelected = false;
                    }
                    else if (item != null)
                        if (item.DataContext != null)
                            cardView.SelectedItem = item.DataContext;
                        else
                            cardView.SelectedItem = item.Content;
                }
            }
            if (cardGroup != null && cardView.IsGrouping)
            {
                CardViewItem item = new CardViewItem();
                item = this as CardViewItem;
                cardView.SelectedItem = this.Header;
                if (item != null)
                {
                    item.IsSelected = true;
                    if (cardView.previousSelectedItem != null && cardView.previousSelectedItem != item)
                    {
                        cardView.previousSelectedItem.IsSelected = false;
                        cardView.previousSelectedItem.IsInEditMode = false;
                        cardView.previousSelectedItem._content.ContentTemplate = cardView.ItemTemplate;
                    }

                    cardView.previousSelectedItem = item;
                    if (!cardView.CanEdit)
                    {
                        item.IsInEditMode = false;
                        item._content.ContentTemplate = cardView.ItemTemplate;
                    }
                }
            }
        }

        protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (cardView != null && cardView.CanEdit)
            {
                IsInEditMode = true;
                this._content.ContentTemplate = cardView.EditItemTemplate;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                if (cardView != null && cardView.CanEdit)
                {
                    IsInEditMode = true;
                    this._content.ContentTemplate = cardView.EditItemTemplate;
                }
            }

            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                if (IsInEditMode)
                {
                    this._content.ContentTemplate = cardView.ItemTemplate;
                    IsInEditMode = false;
                }
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CardViewItem), new UIPropertyMetadata(false));


        

      
        }
}
