// <copyright file="OpacityDockPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// OpacityDockPanel describes the panel in the <see cref="SidePanel"/>.
    /// </summary>
    /// <example>
    /// <para/>This example shows how to use the OpacityDockPanel in XAML. OpacityDockPanel is a part of the <see cref="SidePanel"/>.
    /// <code language="XAML">
    /// <![CDATA[
    /// <Syncfusion:OpacityDockPanel LastChildFill="True" Opacity="{Binding Path=ContentOpacity, RelativeSource={RelativeSource TemplatedParent}}">
    /// <DockPanel.RenderTransform>
    /// <TransformGroup>
    /// <TranslateTransform>
    /// <TranslateTransform.X>
    /// <MultiBinding Converter="{StaticResource SideToCoordinate}" ConverterParameter="XCoordinate" >
    /// <Binding Path="TabStripPlacement" RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="(FrameworkElement.DataContext).(Syncfusion:DockingManager.DesiredWidthInDockedMode)"
    /// RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="ContentRenderTransformX" RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="Width" ElementName="PART_Shadow" />
    /// <Binding Path="SidePanelBorderThickness" RelativeSource="{RelativeSource FindAncestor
    /// , AncestorType={x:Type Syncfusion:DockingManager}}" />
    /// </MultiBinding>
    /// </TranslateTransform.X>
    /// <TranslateTransform.Y>
    /// <MultiBinding Converter="{StaticResource SideToCoordinate}" ConverterParameter="YCoordinate">
    /// <Binding Path="TabStripPlacement" RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="(FrameworkElement.DataContext).(Syncfusion:DockingManager.DesiredHeightInDockedMode)"
    /// RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="ContentRenderTransformY" RelativeSource="{RelativeSource TemplatedParent}" />
    /// <Binding Path="Height" ElementName="PART_Shadow" />
    /// <Binding Path="SidePanelBorderThickness" RelativeSource="{RelativeSource FindAncestor
    /// , AncestorType={x:Type Syncfusion:DockingManager}}" />
    /// </MultiBinding>
    /// </TranslateTransform.Y>
    /// </TranslateTransform>
    /// <ScaleTransform ScaleX="{Binding Path=ContentScaleX, RelativeSource={RelativeSource TemplatedParent}}"
    /// ScaleY="{Binding Path=ContentScaleY, RelativeSource={RelativeSource TemplatedParent}}"
    /// />
    /// </TransformGroup>
    /// </DockPanel.RenderTransform>
    /// </Syncfusion:OpacityDockPanel>
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="DockPanel"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class OpacityDockPanel : DockPanel
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="OpacityDockPanel"/> class.
        /// </summary>
        static OpacityDockPanel()
        {
            OpacityProperty.OverrideMetadata(typeof(OpacityDockPanel), new FrameworkPropertyMetadata(1d, new PropertyChangedCallback(OnOpacityChanged)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when [opacity changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OpacityDockPanel instance = (OpacityDockPanel)d;
            double newOpacity = (double)e.NewValue;

            if (0 == newOpacity)
            {
                instance.Visibility = Visibility.Collapsed;
            }
            else if (Visibility.Collapsed == instance.Visibility)
            {
                instance.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
