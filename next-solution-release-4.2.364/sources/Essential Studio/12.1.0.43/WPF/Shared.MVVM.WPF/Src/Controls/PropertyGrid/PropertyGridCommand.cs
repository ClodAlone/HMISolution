#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.PropertyGrid;


namespace Syncfusion.Windows.Tools.MVVM
{

    #region PropertyGridSelectedObjectChangedCommand
    // PropertyGridSelectedObjectChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSelectedObjectChangedCommand : ControlCommandBase<PropertyGridSelectedObjectChangedCommandBehavior, Syncfusion.Windows.PropertyGrid.PropertyGrid>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSelectedObjectChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.PropertyGrid.PropertyGrid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedObjectChanged += OnEventRaised;
        }
    }

    // PropertyGridSelectedObjectChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridSelectedObjectChangedCommandBehavior<T> : PropertyGridSelectedObjectChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridSortDirectionChangedCommand
    // PropertyGridSortDirectionChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSortDirectionChangedCommand : ControlCommandBase<PropertyGridSortDirectionChangedCommandBehavior, Syncfusion.Windows.PropertyGrid.PropertyGrid>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSortDirectionChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.PropertyGrid.PropertyGrid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SortDirectionChanged += OnEventRaised;
        }
    }

    // PropertyGridSortDirectionChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridSortDirectionChangedCommandBehavior<T> : PropertyGridSortDirectionChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridEnableGroupingChangedCommand
    // PropertyGridEnableGroupingChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridEnableGroupingChangedCommand : ControlCommandBase<PropertyGridEnableGroupingChangedCommandBehavior, Syncfusion.Windows.PropertyGrid.PropertyGrid>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridEnableGroupingChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.PropertyGrid.PropertyGrid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.EnableGroupingChanged += OnEventRaised;
        }
    }

    // PropertyGridEnableGroupingChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridEnableGroupingChangedCommandBehavior<T> : PropertyGridEnableGroupingChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridValueChangedCommand
    // PropertyGridValueChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridValueChangedCommand : ControlCommandBase<PropertyGridValueChangedCommandBehavior, Syncfusion.Windows.PropertyGrid.PropertyGrid>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridValueChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.PropertyGrid.PropertyGrid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, ValueChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.ValueChanged += OnEventRaised;
        }
    }

    // PropertyGridValueChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridValueChangedCommandBehavior<T> : PropertyGridValueChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridSelectedPropertyItemChangedCommand
    // PropertyGridSelectedObjectChangedCommand
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSelectedPropertyItemChangedCommand : ControlCommandBase<PropertyGridSelectedPropertyItemChangedCommandBehavior, Syncfusion.Windows.PropertyGrid.PropertyGrid>
    { }
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridSelectedPropertyItemChangedCommandBehavior : CommandBehaviorBase<Syncfusion.Windows.PropertyGrid.PropertyGrid>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, DependencyPropertyChangedEventArgs e)
        {
            ExecuteCommand();
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void OnTargetAttached()
        {
            TargetObject.SelectedPropertyItemChanged += OnEventRaised;
        }
    }

    // PropertyGridSelectedPropertyChangedCommandBehavior
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridSelectedPropertyChangedCommandBehavior<T> : PropertyGridSelectedPropertyItemChangedCommandBehavior
    { }
    #endregion
}


