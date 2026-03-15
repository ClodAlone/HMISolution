// <copyright file="ChartLegendEditor.xaml.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Data;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using System.Windows.Shapes;
    using System.ComponentModel;

  /// <summary>
  /// Interaction logic for ChartLegendEditor.xaml
  /// </summary>
  /// <exclude/>
    #if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
    #endif
  public partial class ChartLegendEditor : Window
  {
    #region Members
    /// <summary>
    /// Initializes m_iconVisibility
    /// </summary>
    private Visibility m_iconVisibility;

    /// <summary>
    /// Initializes m_checkboxVisibility
    /// </summary>
    private Visibility m_checkboxVisibility;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartLegendEditor"/> class.
    /// </summary>
    public ChartLegendEditor()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Overloaded. Sets <see cref="ChartLegend"/> as window's data context and saves previous legend's state.
    /// </summary>
    /// <param name="legend">The <see cref="ChartLegend"/>.</param>
    public void ShowDialog(ChartLegend legend)
    {
      this.m_iconVisibility = legend.IconVisibility;
      this.m_checkboxVisibility = legend.CheckBoxVisibility;
      this.DataContext = legend;
      SetBinding(FlowDirectionProperty, new Binding("FlowDirection") { Source = legend });
      this.ShowDialog();
    }

    /// <summary>
    /// Cancel click handler.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
      ((ChartLegend)this.DataContext).IconVisibility = this.m_iconVisibility;
      ((ChartLegend)this.DataContext).CheckBoxVisibility = this.m_checkboxVisibility;
      this.DialogResult = false;
    }
  }
}