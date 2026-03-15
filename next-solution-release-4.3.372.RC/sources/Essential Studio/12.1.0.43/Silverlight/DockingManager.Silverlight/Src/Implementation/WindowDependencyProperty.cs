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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Partially calls Window Control,these class contains the dependencey property
    /// </summary>
    public partial class Window :IDisposable
    {
        private Brush _windowBackGround;
        private Brush _windowBorderBrush;
        private Thickness _windowBorderThickness;
        private CornerRadius _windowCornerRadius;
        private Brush _headerBackgroud; 
        private Brush _headerBorderBrush;
        private Brush _optionButtonMouseHoverColor;
        private Brush _optionButtonFillColor;
        private Thickness _captionMargin;
        private FontFamily _captionFontFamily;
        private double _captionFontSize;
        private Brush _captionForeGround;
        private Brush _activeForeground;

        /// <summary>
        /// Gets or sets the BorderBrush of the window.
        /// </summary>
        /// <value>
        /// Default value is Brushes.0xFF, 0x65, 0x93, 0xCF.
        /// </value>
        /// <seealso cref="Brush"/>
        protected internal Brush WindowBorderBrush
        {
            get
            {
                return _windowBorderBrush;
            }
            set
            {
                _windowBorderBrush = value;
                if (value != null)
                {
                    if (windowBorder != null)
                    {
                        windowBorder.BorderBrush = _windowBorderBrush;
                        if (contentBorder != null)
                        {
                            contentBorder.BorderBrush = _windowBorderBrush;
                            contentBorder.BorderThickness = new Thickness(0, 0, 0, 0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the window back ground.
        /// </summary>
        /// <value>The window back ground.</value>
        protected internal Brush WindowBackGround
        {
            get
            {
                return _windowBackGround;
            }
            set
            {
                _windowBackGround = value;
                if (value != null)
                {
                    if (windowBorder != null)
                    {
                        windowBorder.Background = _windowBackGround;                       
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets Border thickness os the Window Control.
        /// This is a Dependency property
        /// </summary>
        /// <value>
        /// Default Thickness is 1
        /// </value>
        protected internal Thickness WindowBorderThickness
        {
            get
            {
                return _windowBorderThickness;
            }
            set
            {
                _windowBorderThickness = value;
                if (value != null)
                {
                    if (windowBorder != null)
                    {
                        windowBorder.BorderThickness = _windowBorderThickness;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the CornerRadius of the Window.
        /// </summary>
        /// <value>
        /// default value is 0
        /// </value>
        protected internal CornerRadius WindowCornerRadius
        {
            get
            {
                return _windowCornerRadius;
            }
            set
            {
                _windowCornerRadius = value;
                if (value != null)
                {
                    if (windowBorder != null)
                    {
                        windowBorder.CornerRadius = _windowCornerRadius;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the window tool tip.
        /// </summary>
        /// <value>The window tool tip.</value>
        public object WindowToolTip
        {
            get
            {
                if (WindowChildElement != null)
                {
                    return ToolTipService.GetToolTip(WindowChildElement);
                }
                else
                {
                    return null;
                }
            }           
        }

        /// <summary>
        /// Gets or sets the HeaderBackgroud of the Window.
        /// </summary>
        /// <value>
        /// default value is 0xFF, 0x65, 0x93, 0xCF
        /// </value>
        protected internal Brush HeaderBackgroud
        {
            get
            {
                return _headerBackgroud;
            }
            set
            {
                _headerBackgroud = value;
                if (value != null)
                {
                    if (captionBar != null)
                    {
                        ((Border)captionBar).Background = _headerBackgroud;
                        if (partInnerGrid != null)
                        {
                            partInnerGrid.Background = _headerBackgroud;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the Header BorderBrush of the Window.
        /// </summary>
        /// <value>
        /// default value is Brushes.transparent
        /// </value>
        protected internal Brush HeaderBorderBrush
        {
            get
            {
                return _headerBorderBrush;
            }
            set
            {
                _headerBorderBrush = value;
                if (value != null)
                {
                    if (captionBar != null)
                    {
                        ((Border)captionBar).BorderBrush = _headerBorderBrush;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Caption ForeGround of the Window.
        /// </summary>
        /// <value>
        /// default value is White
        /// </value>
        protected internal Brush CaptionForeGround
        {
            get
            {
                return _captionForeGround;
            }
            set
            {
                _captionForeGround = value;
                if (value != null)
                {
                    if (caption != null)
                    {
                        caption.Foreground = _captionForeGround;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or Sets the active foreground
        /// </summary>
        protected internal Brush ActiveForeground
        {
            get
            {
                return _activeForeground;
            }
            set
            {
                if (value != null)
                {
                    _activeForeground = value;
                    if (caption != null)
                        caption.Foreground = _activeForeground;
                }
            }
        }


        /// <summary>
        /// Gets or sets the Caption FontSize of the Window.
        /// </summary>
        /// <value>
        /// default value is 12d
        /// </value>
        protected internal double CaptionFontSize
        {
            get
            {
                return _captionFontSize;
            }
            set
            {
                _captionFontSize = value;

                if (value != 0)
                {
                    if (caption != null)
                    {
                        caption.FontSize = _captionFontSize;
                    }
                }
            }
        }

        private double previousWidth;

        /// <summary>
        /// Gets or Sets the previous width of the window when its Maximized state is changed
        /// </summary>
        protected internal double PreviousWidth
        {
            get
            {
                return previousWidth;
            }
            set
            {
                previousWidth = value;
            }
        }

        private double previousHeight;

        /// <summary>
        /// Gets or Sets the previous height of the window when its maximized state is changed
        /// </summary>
        protected internal double PreviousHeight
        {
            get
            {
                return previousHeight;
            }
            set
            {
                previousHeight = value;
            }
        }

        /// <summary>
        /// Gets or Sets the previous float width when its MaximizedState is changed
        /// </summary>
        protected internal double PreviousFloatWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the previous float height when its MaximizedState is changed
        /// </summary>
        protected internal double PreviousFloatHeight
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the  previous left location when its MaximizedState is changed
        /// </summary>
        protected internal double PreviousLeftLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the  previous top location when its MaximizedState is changed
        /// </summary>
        protected internal double PreviousTopLocation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Header's font family of the Window.
        /// </summary>
        /// <value>
        /// default value is tahoma
        /// </value>
        protected internal FontFamily CaptionFontFamily
        {
            get
            {
                return _captionFontFamily;
            }
            set
            {
                _captionFontFamily = value;
                if (value != null)
                {
                    if (caption != null)
                    {
                        caption.FontFamily = _captionFontFamily;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Margin of the Window's inner contnet.
        /// </summary>
        /// <value>
        /// default value is 5
        /// </value>
        protected internal Thickness CaptionMargin
        {
            get
            {
                return _captionMargin;
            }
            set
            {
                _captionMargin = value;
                if (value != null)
                {
                    if (caption != null)
                    {
                        caption.Margin = _captionMargin;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Option Button FillColor of the Window.
        /// </summary>
        /// <value>
        /// default value is black
        /// </value>
        protected internal Brush OptionButtonFillColor
        {
            get
            {
                return _optionButtonFillColor;
            }
            set
            {
                _optionButtonFillColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Option Button MouseHoverColor
        /// </summary>
        /// <value>
        /// default value is Black
        /// </value>
        protected internal Brush OptionButtonMouseHoverColor
        {
            get
            {
                return _optionButtonMouseHoverColor;
            }
            set
            {
                _optionButtonMouseHoverColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Context menu Template of the Window.
        /// </summary>
        /// <value>
        /// default value is null
        /// </value>
        public Style MenuButtonTemplate
        {
            get
            {
                return (Style)GetValue(MenuButtonTemplateProperty);
            }

            set
            {
                SetValue(MenuButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MenuButtonTemplate"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty MenuButtonTemplateProperty =
           DependencyProperty.Register("MenuButtonTemplate", typeof(Style), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the Maximize Button Template of the Window.
        /// </summary>
        /// <value>
        /// default value is null
        /// </value>
        public Style MaximizeButtonTemplate
        {
            get
            {
                return (Style)GetValue(MaximizeButtonTemplateProperty);
            }

            set
            {
                SetValue(MaximizeButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MenuButtonTemplate"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty MaximizeButtonTemplateProperty =
           DependencyProperty.Register("MaximizeButtonTemplate", typeof(Style), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the context menu style
        /// </summary>
        public Style ContextMenuStyle
        {
            get
            {
                return (Style)GetValue(ContextMenuStyleProperty);
            }
            set
            {
                SetValue(ContextMenuStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or Sets the ContextMenuItem Style
        /// </summary>
        public Style ContextMenuItemStyle
        {
            get
            {
                return (Style)GetValue(ContextMenuItemStyleProperty);
            }
            set
            {
                SetValue(ContextMenuItemStyleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the ContextMenuStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextMenuStyleProperty =
            DependencyProperty.Register("ContextMenuStyle", typeof(Style), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ContextMenuItemStyle dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty ContextMenuItemStyleProperty =
            DependencyProperty.Register("ContextMenuItemStyle", typeof(Style), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header template.
        /// </summary>
        /// <value>The header template.</value>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)GetValue(HeaderTemplateProperty);
            }

            set
            {
                SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the color of the menu highlighting.
        /// </summary>
        /// <value>The color of the menu highlighting.</value>
        public Brush MenuHighlightingColor
        {
            get
            {
                return (Brush)GetValue(MenuHighlightingColorProperty);
            }

            set
            {
                SetValue(MenuHighlightingColorProperty, value);
            }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty MenuHighlightingColorProperty =
            DependencyProperty.Register("MenuHighlightingColor", typeof(Brush), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public string Header
        {
            get
            {
                return (string)GetValue(HeaderProperty);
            }

            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Identifies the HeaderTemplate dependency property of the <see cref="DockingManager"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the CloseButton Template of the Window.
        /// </summary>
        /// <value>
        /// default value is null
        /// </value>
        public Style CloseButtonTemplate
        {
            get
            {
                return (Style)GetValue(CloseButtonTemplateProperty);
            }

            set
            {
                SetValue(CloseButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CloseButtonTemplate"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty CloseButtonTemplateProperty =
               DependencyProperty.Register("CloseButtonTemplate", typeof(Style), typeof(Window), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the dockButton Template of the Window.
        /// </summary>
        /// <value>
        /// default value is null
        /// </value>
        public Style AwlButtonTemplate
        {
            get
            {
                return (Style)GetValue(AwlButtonTemplateProperty);
            }

            set
            {
                SetValue(AwlButtonTemplateProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="AwlButtonTemplate"/> dependency property key.
        /// </summary>
        public static readonly DependencyProperty AwlButtonTemplateProperty =
            DependencyProperty.Register("AwlButtonTemplate", typeof(Style), typeof(Window), new PropertyMetadata(null));

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            
        }

        #endregion
    }
}
