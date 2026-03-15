// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Specifies drag directions.
    /// </summary>
    public enum DragDirection
    {
        /// <summary>
        /// Up drag direction.
        /// </summary>
        Up = 0,
        
        /// <summary>
        /// Down drag direction.
        /// </summary>
        Down,
        
        /// <summary>
        /// There is no drag direction.
        /// </summary>
        None
    }
    
    /// <summary>
    /// Specifies visual styles used for drawing <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> control.
    /// </summary>
    public enum VisualStyle
    {
        /// <summary>
        /// VisualStudio2005 visual style.
        /// </summary>
        Default = 0,
        
        /// <summary>
        /// Office2003 visual style.
        /// </summary>
        Office2003 = 1,
        
        /// <summary>
        /// Office2007 Blue visual style.
        /// </summary>
        Office2007Blue = 2,
        
        /// <summary>
        /// Office2007 Silver visual style.
        /// </summary>
        Office2007Silver = 3,
        
        /// <summary>
        /// Office2007 Black visual style.
        /// </summary>
        Office2007Black = 4,
        
        /// <summary>
        /// Blend visual style.
        /// </summary>
        Blend = 5,
        
        /// <summary>
        /// Luna NormalColor visual style.
        /// </summary>
        LunaNormalColor = 6,
        
        /// <summary>
        /// Luna Homestead visual style.
        /// </summary>
        LunaHomestead = 7,
        
        /// <summary>
        /// Luna Metallic visual style.
        /// </summary>
        LunaMetallic = 8,
        
        /// <summary>
        /// Royale NormalColor visual style.
        /// </summary>
        RoyaleNormalColor = 9,
        
        /// <summary>
        /// Zune NormalColor visual style.
        /// </summary>
        ZuneNormalColor = 10,
        
        /// <summary>
        /// Aero NormalColor visual style.
        /// </summary>
        AeroNormalColor = 11
    }
    
    /// <summary>
    /// Specifies visual modes of <see cref="Syncfusion.Windows.Tools.Controls.GroupBar"/> control.
    /// </summary>
    public enum VisualMode
    {
        /// <summary>
        /// Default visual mode. Only one item can be expanded. 
        /// </summary>
        Default,
        
        /// <summary>
        /// Multiple expansion visual mode. More than one item can be expanded.
        /// </summary>
        MultipleExpansion,
        
        /// <summary>
        /// Stack visual mode. Only one item can be expanded. Items are organized in stack-like mode.
        /// </summary>
        StackMode
    }
    
    /// <summary>
    /// Specifies cursor types for GroupBar items and GroupView items.
    /// </summary>
    public enum ItemCursorType
    {
        /// <summary>
        /// Default cursor type.
        /// </summary>
        Default,
        
        /// <summary>
        /// Hand cursor type.
        /// </summary>
        Hand
    }
    
    /// <summary>
    /// Specifies the collapse/expand direction of the group bar.
    /// </summary>
    public enum CollapseDirection
    {
        /// <summary>
        /// Left collapse/expand direction.
        /// </summary>
        Left,
        
        /// <summary>
        /// Right collapse/expand direction.
        /// </summary>
        Right
    }
    
    /// <summary>
    /// Specifies the animation type.
    /// </summary>
    public enum AnimationsType
    {
        /// <summary>
        /// There is no animation.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Slide animation.
        /// </summary>
        Slide = 1,
        
        /// <summary>
        /// Fade animation.
        /// </summary>
        Fade = 2
    }
    
    /// <summary>
    /// Specifies resize mode for the Navigation pane's popup. 
    /// </summary>
    public enum PopupResizeDirection
    {
        /// <summary>
        /// Size of the popup cannot be changed.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Horizontal resizing of the popup is available.
        /// </summary>
        Horizontal = 1,
        
        /// <summary>
        /// Vertical resizing of the popup is available.
        /// </summary>
        Vertical = 2,
        
        /// <summary>
        /// popup can be resized in any direction.
        /// </summary>
        Both = 3
    }
}

