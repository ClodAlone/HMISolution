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
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Interactive code for Strip Line
    /// </summary>
    [
    TemplatePart(Name = "PART_ParentContent", Type = typeof(ContentPresenter)),
    TemplatePart(Name = "PART_InnerContent", Type = typeof(ContentPresenter))
    ]
    public class StripLine : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StripLine"/> class.
        /// </summary>
        public StripLine()
        {
            DefaultStyleKey = GetType();
        }

        #endregion

        #region Dependency Registration

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(StripLine), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for ContentTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(StripLine), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for Type.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(StriplineType), typeof(StripLine), new PropertyMetadata(StriplineType.Regular));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the content template.
        /// </summary>
        /// <value>The content template.</value>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public StriplineType Type
        {
            get { return (StriplineType)GetValue(TypeProperty); }
            internal set { SetValue(TypeProperty, value); }
        }

        #endregion

        #region Internal Properties

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        internal Point Position { get; set; }

        /// <summary>
        /// Gets or sets the parent presenter.
        /// </summary>
        /// <value>The parent presenter.</value>
        internal ContentPresenter ParentPresenter { get; set; }

        /// <summary>
        /// Gets or sets the inner presenter.
        /// </summary>
        /// <value>The inner presenter.</value>
        internal ContentPresenter InnerPresenter { get; set; }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ParentPresenter = (ContentPresenter)GetTemplateChild("PART_ParentContent");
            this.InnerPresenter = (ContentPresenter)GetTemplateChild("PART_InnerContent");
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="T:System.Windows.Controls.Control"/> object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.Type == StriplineType.Absolute)
            {
                if (this.InnerPresenter != null)
                {
#if !SILVERLIGHT
                    this.InnerPresenter.LayoutTransform = new RotateTransform(0);
#else
                    this.InnerPresenter.Projection = new PlaneProjection() { RotationZ = 0 };
#endif
                }      
            }
            return base.ArrangeOverride(arrangeBounds);
        }
#if SILVERLIGHT

        /// <summary>
        /// Provides the behavior for the Measure pass of Silverlight layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity"/>) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if(this.ParentPresenter!=null)
            {
                this.ParentPresenter.Measure(new Size(double.MaxValue, double.MaxValue));
                double size = Math.Max(this.ParentPresenter.DesiredSize.Height, this.ParentPresenter.DesiredSize.Width);
                this.ParentPresenter.Height = size;
                this.ParentPresenter.Width = size;
            }
            return base.MeasureOverride(availableSize);
        }
#endif

        #endregion
    }
}
