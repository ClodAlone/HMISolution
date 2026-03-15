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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.MVVM
{

    #region PropertyGridSelectedObjectChangedCommand
    /// <summary>
    /// PropertyGridSelectedObjectChangedCommand
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

    /// <summary>
    /// PropertyGridSelectedObjectChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridSelectedObjectChangedCommandBehavior<T> : PropertyGridSelectedObjectChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridSortDirectionChangedCommand
    /// <summary>
    /// PropertyGridSortDirectionChangedCommand
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

    /// <summary>
    /// PropertyGridSortDirectionChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridSortDirectionChangedCommandBehavior<T> : PropertyGridSortDirectionChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridEnableGroupingChangedCommand
    /// <summary>
    /// PropertyGridEnableGroupingChangedCommand
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

    /// <summary>
    /// PropertyGridEnableGroupingChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridEnableGroupingChangedCommandBehavior<T> : PropertyGridEnableGroupingChangedCommandBehavior
    { }
    #endregion

    #region PropertyGridValueChangedCommand
    /// <summary>
    /// PropertyGridValueChangedCommand
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
        protected virtual void OnEventRaised(object sender, Syncfusion.Windows.PropertyGrid.ValueChangedEventArgs e)
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

    /// <summary>
    /// PropertyGridValueChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyGridValueChangedCommandBehavior<T> : PropertyGridValueChangedCommandBehavior
    { }
    #endregion
}


