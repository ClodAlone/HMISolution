/* Copyright © Northwoods Software Corporation, 2008-2015. All Rights Reserved. */

using System.Windows;
using System.Windows.Controls;
using Northwoods.GoXam;

namespace EntityRelationship {
  public partial class EntityForm : UserControl {
    public EntityForm() {
      InitializeComponent();
    }

    // the Close button
    private void Button_Click(object sender, RoutedEventArgs e) {
      Node popup = Part.FindAncestor<Node>(e.OriginalSource as UIElement);
      if (popup != null) popup.Visible = false;
    }
  }
}
