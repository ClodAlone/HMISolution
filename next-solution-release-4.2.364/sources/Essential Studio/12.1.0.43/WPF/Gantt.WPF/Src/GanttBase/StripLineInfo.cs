#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Controls.Gantt
{
    public class StripLineInfo : NotificationObject
    {
        #region Constructor

        public StripLineInfo()
        {
            this.RepeatUpto = DateTime.MaxValue;
            _horizontalContentAlignment = HorizontalAlignment.Center;
        }

        #endregion

        #region Private Members

        private object _content;
        private Brush _background;
        private HorizontalAlignment _horizontalContentAlignment;
        private DataTemplate _contentTemplate;
        private VerticalAlignment _verticalContentAlignment;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }


        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get 
            { 
                return _background; }
            set 
            { 
                _background = value; 
                RaisePropertyChanged("Background"); 
            }
        }

        /// <summary>
        /// Gets or sets the vertical content alignment.
        /// </summary>
        /// <value>The vertical content alignment.</value>
        public VerticalAlignment VerticalContentAlignment
        {
            get 
            { 
                return _verticalContentAlignment; 
            }
            set 
            { 
                _verticalContentAlignment = value; 
                RaisePropertyChanged("VerticalContentAlignment"); 
            }
        }

        /// <summary>
        /// Gets or sets the horizontal content alignment.
        /// </summary>
        /// <value>The horizontal content alignment.</value>
        public HorizontalAlignment HorizontalContentAlignment
        {
            get
            {
                return _horizontalContentAlignment;
            }
            set
            {
                _horizontalContentAlignment = value;
                RaisePropertyChanged("HorizontalContentAlignment");
            }
        }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        public DataTemplate ContentTemplate
        {
            get 
            { 
                return _contentTemplate; 
            }

            set 
            { 
                _contentTemplate = value; 
                RaisePropertyChanged("ContentTemplate");
            }
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the repeat behavior.
        /// </summary>
        /// <value>The repeat behavior.</value>
        public Repeat RepeatBehavior { get; set; }

        /// <summary>
        /// Gets or sets the repeat for.
        /// </summary>
        /// <value>The repeat for.</value>
        public int RepeatFor { get; set; }

        /// <summary>
        /// Gets or sets the repeat upto.
        /// </summary>
        /// <value>The repeat upto.</value>
        public DateTime RepeatUpto { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public Style Style { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public StriplineType Type { get; set; }
#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the style selector.
        /// </summary>
        /// <value>The style selector.</value>
        public StyleSelector StyleSelector { get; set; }

        /// <summary>
        /// Gets or sets the control template selector.
        /// </summary>
        /// <value>The control template selector.</value>
        public DataTemplateSelector ContentTemplateSelector { get; set; }
#endif

        #endregion

    }
}
