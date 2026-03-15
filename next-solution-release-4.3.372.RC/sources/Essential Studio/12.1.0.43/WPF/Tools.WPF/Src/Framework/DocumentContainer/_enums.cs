// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Document's switch representation mode of the document container
    /// </summary>
    public enum SwitchMode
    {
        /// <summary>
        /// Simple switch.
        /// </summary>
        Immediate,
        
        /// <summary>
        /// Shows list with items.
        /// </summary>
        List,
        
        /// <summary>
        /// Shows gallery with items.
        /// </summary>
        QuickTabs,
        
        /// <summary>
        /// Shows like VS2005 action.
        /// </summary>
        VS2005,
        
        /// <summary>
        /// Shows like Vista flip actions.
        /// </summary>
        VistaFlip,

        /// <summary>
        /// No Switch.
        /// </summary>
        None
    }
    
    /// <summary>
    /// Document representation mode of the document container.
    /// </summary>
    public enum DocumentContainerMode
    {
        /// <summary>
        /// Multiple documents interface mode.
        /// </summary>
        MDI,
        
        /// <summary>
        /// Tabbed documents interface mode.
        /// </summary>
        TDI
    }
    
    /// <summary>
    /// Behavior of the button that represent some commands that are not allowed.
    /// </summary>
    public enum DisabledButtonsBehavior
    {
        /// <summary>
        /// Shows button in a disabled state.
        /// </summary>
        Disable,
        
        /// <summary>
        /// Makes disabled button invisible, but does not change the layout - button will look as an empty space.
        /// </summary>
        Hide,
        
        /// <summary>
        /// Completely hides the button.
        /// </summary>
        Collapse
    }
    
    /// <summary>
    /// State of the MDI window.
    /// </summary>
    public enum MDIWindowState
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal,
        
        /// <summary>
        /// Minimized state.
        /// </summary>
        Minimized,

        Maximized,
    }
    
    /// <summary>
    /// Present keyboard mode.
    /// </summary>
    public enum KeyboardOverrideMode
    {
        /// <summary>
        /// No action.
        /// </summary>
        None,
        
        /// <summary>
        /// Window move action.
        /// </summary>
        WindowMove,
        
        /// <summary>
        /// Window resize action.
        /// </summary>
        WindowResize
    }
    
    /// <summary>
    /// Presents switch direction.
    /// </summary>
    public enum SwitchDirection
    {
        /// <summary>
        /// Switches to use Tab key.
        /// </summary>
        Tab,
        
        /// <summary>
        /// Switches to use Left\Right key.
        /// </summary>
        Horizontal,
        
        /// <summary>
        /// Switches to use Up\Down key.
        /// </summary>
        Vertical
    }
    
    /// <summary>
    /// Presents properties mode for persist state.
    /// </summary>
    public enum PropertiesMode
    {
        /// <summary>
        /// Presents main properties.
        /// </summary>
        Main,
        
        /// <summary>
        /// Present children properties.
        /// </summary>
        Child
    }
    
    /// <summary>
    /// Presents MDI layout mode.
    /// </summary>
    public enum MDILayout
    {
        /// <summary>
        /// Presents cascade mode.
        /// </summary>
        Cascade,
        
        /// <summary>
        /// Presents horizontal mode.
        /// </summary>
        Horizontal,
        
        /// <summary>
        /// Presents vertical mode.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Represents split mode
    /// </summary>
    internal enum SplitMode
    {
        /// <summary>
        /// No splitting SplitMode
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Horizontal splitting
        /// </summary>
        Horizontal = 2,
        
        /// <summary>
        /// Vertical splitting
        /// </summary>
        Vertical = 4
    }
    
    /// <summary>
    /// Side which should resized in TDIContainerPanel
    /// </summary>
    internal enum ResizedSide
    {
        /// <summary>
        /// Left side TDIContainerPanel
        /// </summary>
        Left = 0,
        
        /// <summary>
        /// Top side TDIContainerPanel
        /// </summary>
        Top = 2,
        
        /// <summary>
        /// Right side TDIContainerPanel
        /// </summary>
        Right = 4,
        
        /// <summary>
        /// Bottom side TDIContainerPanel
        /// </summary>
        Bottom = 8
    }
    
    /// <summary>
    /// MDI Border of the Document Container
    /// </summary>
    internal enum MDIBorder
    {
        /// <summary>
        /// Left side  MDIBorder
        /// </summary>
        Left,
        
        /// <summary>
        /// Left topside MDIBorder
        /// </summary>
        LeftTop,
        
        /// <summary>
        /// Top side MDIBorder
        /// </summary>
        Top,
        
        /// <summary>
        /// Right top side MDIBorder
        /// </summary>
        RightTop,
        
        /// <summary>
        /// Right side MDIBorder
        /// </summary>
        Right,
        
        /// <summary>
        /// Right bottom MDIBorder
        /// </summary>
        RightBottom,
        
        /// <summary>
        /// bottom side MDIBorder
        /// </summary>
        Bottom,
        
        /// <summary>
        /// Left bottom side MDIBorder
        /// </summary>
        LeftBottom,
        
        /// <summary>
        /// center side MDIBorder
        /// </summary>
        Center,
        
        /// <summary>
        /// header side MDIBorder
        /// </summary>
        Header,
        
        /// <summary>
        /// outside MDIBorder
        /// </summary>
        Outside
    }
}