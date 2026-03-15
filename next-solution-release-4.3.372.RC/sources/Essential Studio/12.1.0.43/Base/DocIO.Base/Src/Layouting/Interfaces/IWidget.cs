#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using Syncfusion.DocIO.DLS;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
using System.Drawing;
#endif

namespace Syncfusion.Layouting
{
    #region Interface IWidget
    /// <summary>
    /// Represents a base Widget interface
    /// </summary>
  internal interface IWidget
  {
#if !(SILVERLIGHT || WP)
        /// <summary>
        /// Gets layout info.
        /// </summary>
        ILayoutInfo LayoutInfo { get; }

        /// <summary>
        /// Draws the specified dc.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="ltWidget">The lt widget.</param>
        void Draw(DrawingContext dc, LayoutedWidget ltWidget);
        /// <summary>
        /// Initializing LayoutInfo value to null
        /// </summary>
        void InitLayoutInfo();
    
#endif
  }
    #endregion

    #region Interface IWidgetContainer
    /// <summary>
    /// Represents a collection of Widget objects.
    /// </summary>
  internal interface IWidgetContainer : IWidget
  {
      /// <summary>
      /// Gets count of child widgets.
      /// </summary>
      int Count { get; }

      /// <summary>
      /// Gets child widget by index.
      /// </summary>
      IWidget this[int index] { get; }
      /// <summary>
      /// Get ChildWidgets
      /// </summary>
      EntityCollection WidgetInnerCollection { get; }
  }
    #endregion

    #region Interface ILeafWidget
#if !SILVERLIGHT && !WP
    /// <summary>
    /// Represent a widget that can MEASURE self.
    /// </summary>
    internal interface ILeafWidget : IWidget
    {
        /// <summary>
        /// Measures self size. 
        /// </summary>
        /// <param name="graphics"></param>
        /// <returns></returns>
        SizeF Measure(DrawingContext dc);
    }
#endif
    #endregion

    #region interface ISplitLeafWidget
#if !SILVERLIGHT && !WP
    /// <summary>
    /// Represents a LEAF Widget that can SPLIT self.
    /// </summary>
    internal interface ISplitLeafWidget : ILeafWidget
    {
        /// <summary>
        /// Splits the size of the by.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="size">The size.</param>
        /// <param name="clientWidth">The clientWidth.</param>
        /// <returns></returns>
        ISplitLeafWidget[] SplitBySize(DrawingContext dc, SizeF size, float clientWidth, float clientActiveAreaWidth, ref bool isLastWordFit);
    }
#endif
    #endregion
}