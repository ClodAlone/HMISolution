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

#region file using directives
using System;
using Syncfusion.DocIO.DLS;
using Syncfusion.Layouting;
#if !SILVERLIGHT && !WP
using Syncfusion.DocIO.Rendering;
#endif
#endregion

namespace Syncfusion.DocIO.DLS
{
    /// <summary>
    /// The base implementation of IWidget interface
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class WidgetBase
    : Entity,
      IWidget
    {
#if !SILVERLIGHT && !WP
        #region Class members
        /// <summary>
        /// 
        /// </summary>
        internal ILayoutInfo m_layoutInfo;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets layout info.
        /// </summary>
        ILayoutInfo IWidget.LayoutInfo
        {
            get
            {
                if (m_layoutInfo == null)
                {
                    CreateLayoutInfo();
                }
                return m_layoutInfo;
            }
        }
        #endregion
#endif

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetBase"/> class.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="owner">The owner.</param>
        public WidgetBase(WordDocument doc, Entity owner)
            : base(doc, owner)
        {
        }
        #endregion

#if !SILVERLIGHT && !WP
        #region Class public methods
        /// <summary>
        /// Imlementation of Draw method of IWidget interface .
        /// </summary>
        /// <param name="cg"></param>
        /// <param name="ltWidget"></param>
        void IWidget.Draw(DrawingContext dc, LayoutedWidget ltWidget)
        {
            DrawImpl(dc, ltWidget);
        }
        /// <summary>
        /// Initializing layout info to null
        /// </summary>
        void IWidget.InitLayoutInfo()
        {
            m_layoutInfo = null;
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Draw widget to graphics.
        /// </summary>
        internal virtual void DrawImpl(DrawingContext dc, LayoutedWidget ltWidget)
        {
#if DEBUG_RENDERING      
      Color wColor = Color.Green;
      
      if( this is Document )
      {
        wColor = Color.Green;
      }
      else if( this is Section )
      {
        wColor = Color.BlueViolet;
      }
      else if( this is Paragraph )
      {
        wColor = Color.Red;
      }
      else if( this is Picture )
      {
        wColor = Color.Violet;
      }
      else if( this is ParagraphItem )
      {
        wColor = Color.Gray;
      }

      (cg as DLSGraphics).DrawBounds( wColor, ltWidget.Bounds );
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        protected abstract void CreateLayoutInfo();
        #endregion
#endif
    }
    /// <summary>
    /// The base implementation of IWidgetContainer interface
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public abstract class WidgetContainer
    : WidgetBase,
      IWidgetContainer
    {
        #region Class properties

        /// <summary>
        /// Gets count of child widgets.
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return WidgetCollection.Count;
            }
        }
        /// <summary>
        /// Gets child widget by index.
        /// </summary>
        IWidget IWidgetContainer.this[int index]
        {
            get
            {
                return WidgetCollection[index] as IWidget;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected abstract IEntityCollectionBase WidgetCollection
        {
            get;
        }
        /// <summary>
        /// Get Childwidgets
        /// </summary>
        public EntityCollection WidgetInnerCollection
        {
            get
            {
                return WidgetCollection as EntityCollection;
            }
        }
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetContainer"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="owner">The owner.</param>
        public WidgetContainer(WordDocument doc, Entity owner)
            : base(doc, owner)
        {
        }
        #endregion
    }
}
