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

    #region BrushSelectorSelectedBrushChangedCommand
    
    /// <summary>
    /// BrushSelectorSelectedBrushChangedCommand
    /// </summary>
    public class BrushSelectorSelectedBrushChangedCommand : ControlCommandBase<BrushSelectorSelectedBrushChangedCommandBehavior, BrushSelector>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class BrushSelectorSelectedBrushChangedCommandBehavior : CommandBehaviorBase<BrushSelector>
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
            TargetObject.SelectedBrushChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// BrushSelectorSelectedBrushChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BrushSelectorSelectedBrushChangedCommandBehavior<T> : BrushSelectorSelectedBrushChangedCommandBehavior
    { }
    #endregion

    #region BrushSelectorBrushModeChangedCommand
    
    /// <summary>
    /// BrushSelectorBrushModeChangedCommand
    /// </summary>
    public class BrushSelectorBrushModeChangedCommand : ControlCommandBase<BrushSelectorBrushModeChangedCommandBehavior, BrushSelector>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class BrushSelectorBrushModeChangedCommandBehavior : CommandBehaviorBase<BrushSelector>
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
            TargetObject.BrushModeChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// BrushSelectorBrushModeChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BrushSelectorBrushModeChangedCommandBehavior<T> : BrushSelectorBrushModeChangedCommandBehavior
    { }
    #endregion

    #region BrushSelectorEnableGradientToSolidSwitchChangedCommand
    
    /// <summary>
    /// BrushSelectorEnableGradientToSolidSwitchChangedCommand
    /// </summary>
    public class BrushSelectorEnableGradientToSolidSwitchChangedCommand : ControlCommandBase<BrushSelectorEnableGradientToSolidSwitchChangedCommandBehavior, BrushSelector>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class BrushSelectorEnableGradientToSolidSwitchChangedCommandBehavior : CommandBehaviorBase<BrushSelector>
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
            TargetObject.EnableGradientToSolidSwitchChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// BrushSelectorEnableGradientToSolidSwitchChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BrushSelectorEnableGradientToSolidSwitchChangedCommandBehavior<T> : BrushSelectorEnableGradientToSolidSwitchChangedCommandBehavior
    { }
    #endregion

    #region BrushSelectorVisualizationStyleChangedCommand
    
    /// <summary>
    /// BrushSelectorVisualizationStyleChangedCommand
    /// </summary>
    public class BrushSelectorVisualizationStyleChangedCommand : ControlCommandBase<BrushSelectorVisualizationStyleChangedCommandBehavior, BrushSelector>
    { }

    /// <summary>
    /// 
    /// </summary>
    public class BrushSelectorVisualizationStyleChangedCommandBehavior : CommandBehaviorBase<BrushSelector>
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
            TargetObject.VisualizationStyleChanged += OnEventRaised;
        }
    }

    
    /// <summary>
    /// BrushSelectorVisualizationStyleChangedCommandBehavior
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BrushSelectorVisualizationStyleChangedCommandBehavior<T> : BrushSelectorVisualizationStyleChangedCommandBehavior
    { }
    #endregion
   
    


}


