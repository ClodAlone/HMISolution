#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Tools.Enums;

namespace Syncfusion.Windows.Forms.Tools.Renderers
{
    /// <summary>
    /// gets or sets Draw state
    /// Create a class implementing this interface for each custom Renderer and 
    /// add there members and properties representing SplitContainerAdv properties 
    /// you want to save in per-instance basis (s.a. Orientation, BackgroundColor.).
    /// </summary>
    public interface IRendererInfo
    {
        DrawState DrawState { get; set; }
    }

    /// <summary>
    /// Create a class derived from Renderer, implementing this interface, to make 
    /// your own renderers for custom splitter styles and behavior.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Creates RendererInfo instance and initializes it with default values.
        /// </summary>
        /// <param name="instance">An instance to initialize</param>
        /// <param name="ri">A RendererInfo instance to take parameters from</param>
        /// <returns>returns render info</returns>
        IRendererInfo UpdateRendererInfo(SplitContainerAdv instance, IRendererInfo ri);

        /// <summary>
        /// Draws splitter according to settings specified in RendererInfo.
        /// </summary>
        /// <param name="e">Drawing context</param>
        /// <param name="ri">An instance of the needed RendererInfo to retrieve drawing settings from.</param>
        /// <param name="bounds">Bounds to draw within.</param>
        void Draw(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a background.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a background while under mouse cursor.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawHotBackground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a thumbnail.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a thumbnail while under mouse cursor.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawHotThumbnail(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a foreground.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Draws a foreground while under mouse cursor.
        /// </summary>
        /// <param name="e">Current paint context.</param>
        /// <param name="ri">An instance of RendererInfo, which is responsible for this SplitContainerAdv drawing.</param>
        /// <param name="bounds">Within these bounds we should draw.</param>
        void DrawHotForeground(PaintEventArgs e, IRendererInfo ri, Rectangle bounds);

        /// <summary>
        /// Gets theme settings appropriate to the current theme.
        /// </summary>
        /// <param name="instance">An instance of SplitContainerAdv container, which owns this renderer.</param>
        /// <param name="bInit"> Flag of current mode. If mode is initialization than properties will not be set. </param>
        /// <returns>Returns renderer info</returns>
        IRendererInfo GetAppropriateThemeSettings(SplitContainerAdv instance, bool bInit);

        /// <summary>
        /// Compare current value with default value
        /// </summary>
        /// <param name="property"> property identifier </param>
        /// <param name="value"> current value </param>
        /// <returns> true if value is equal and false in other case</returns>
        bool CompareWithDefaultValue(RendererProperty property, object value);

        /// <summary>
        /// Gets default value for some property
        /// </summary>
        /// <param name="property">Rendererer property</param>
        /// <returns>Returns default value</returns>
        object GetDefaultValue(RendererProperty property);
    }
}