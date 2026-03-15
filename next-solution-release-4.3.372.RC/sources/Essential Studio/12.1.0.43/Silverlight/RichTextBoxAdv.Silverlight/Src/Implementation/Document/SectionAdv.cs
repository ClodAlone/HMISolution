#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections.Generic;

#if !WPF
using System.Windows.Browser;
#endif

namespace Syncfusion.Windows.Tools.Controls
{
    [ContentProperty("Blocks")]
    public class SectionAdv : DependencyObject
    {
        internal DocumentAdv Document;
        private BlockCollection<BlockAdv> blocks;
        private LayoutViewer layoutviewer = null;

        /// <summary>
        /// Initializes the new instance of section
        /// </summary>
        public SectionAdv()
        {
            blocks = new BlockCollection<BlockAdv>();
        }

        /// <summary>
        /// Gets the Block Collection
        /// </summary>
        public BlockCollection<BlockAdv> Blocks
        {
            get
            {
                return blocks;
            }
        }

        /// <summary>
        /// Gets or Sets the page size
        /// </summary>
        public Size PageSize
        {
            get
            {
                return (Size)GetValue(PageSizeProperty);
            }
            set
            {
                SetValue(PageSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the page size 817, 1154
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(Size), typeof(SectionAdv), new PropertyMetadata(new Size(817, 1020)));

        /// <summary>
        /// Gets or Sets the page content margin
        /// </summary>
        public Thickness PageContentMargin
        {
            get
            {
                return (Thickness)GetValue(PageContentMarginProperty);
            }
            set
            {
                SetValue(PageContentMarginProperty, value);
            }
        }

        internal LayoutViewer LayoutViewer
        {
            get
            {
                if (Document != null)
                {
                    return Document.OwnerControl.Viewer;
                }
                return null;
            }
        }

        /// <summary>
        /// Registers page content margin dependency property
        /// </summary>
        public static readonly DependencyProperty PageContentMarginProperty = DependencyProperty.Register("PageContentMargin", typeof(Thickness), typeof(SectionAdv), new PropertyMetadata(new Thickness(80)));

        public Brush PageBackground
        {
            get { return (Brush)GetValue(PageBackgroundProperty); }
            set { SetValue(PageBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageBackgroundProperty =DependencyProperty.Register("PageBackground", typeof(Brush), typeof(SectionAdv), new PropertyMetadata(new SolidColorBrush(Colors.White),new PropertyChangedCallback(OnPageBackgroundChanged)));

        
        public Brush PageShadowBackground
        {
            get { return (Brush)GetValue(PageShadowBackgroundProperty); }
            set { SetValue(PageShadowBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageShadowBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageShadowBackgroundProperty =
            DependencyProperty.Register("PageShadowBackground", typeof(Brush), typeof(SectionAdv), new PropertyMetadata(new SolidColorBrush(Colors.Gray), new PropertyChangedCallback(OnPageShadowBackgroundChanged)));



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnPageBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SectionAdv section = dependencyObject as SectionAdv;
            section.OnPageBackgroundChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPageBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (LayoutViewer != null)
                {
                    foreach (PageAdv page in LayoutViewer.Pages)
                    {
                        if (page.Section == this)
                        {
                            page.Background = (Brush)e.NewValue;
                        }
                    }
                }
            }
        }

          /// <summary>
        /// 
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="args"></param>
        protected static void OnPageShadowBackgroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            SectionAdv section = dependencyObject as SectionAdv;
            section.OnPageShadowBackgroundChanged(args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPageShadowBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                if (LayoutViewer != null)
                {
                    foreach (PageAdv page in LayoutViewer.Pages)
                    {
                        if (page.Section == this)
                        {
                            page.ShadowBackground = (Brush)e.NewValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void MeasureElements()
        {
            foreach (BlockAdv block in Blocks)
            {
                block.MeasureElements();
                block.LinkElementBoxes();
            }
        }

        /// <summary>
        /// Returns the next section
        /// </summary>
        /// <returns></returns>
        public SectionAdv GetNextSection()
        {
            if (Document != null)
            {
                int index = Document.Sections.IndexOf(this) + 1;
                if (Document.Sections.Count != index)
                    return Document.Sections[index];
            }

            return null;
        }

        /// <summary>
        /// Returns the previous section
        /// </summary>
        /// <returns></returns>
        public SectionAdv GetPreviousSection()
        {
            if (Document != null)
            {
                int index = Document.Sections.IndexOf(this) - 1;
                if (index >= 0)
                    return Document.Sections[index];
            }

            return null;
        }
    }
}
