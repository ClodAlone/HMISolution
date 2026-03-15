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
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class RibbonCommandProvider : DependencyObject, ICommandSource
    {
        /// <summary>
        /// Command associated with provider.
        /// </summary>
        private ICommand m_command;

        /// <summary>
        /// CommandParameter associated with command.
        /// </summary>
        private object m_commandParameter;

        /// <summary>
        /// Gets or sets a command associated with provider.
        /// </summary>
        /// <seealso cref="ICommand"/>
        public ICommand Command
        {
            get
            {
                return m_command;
            }

            set
            {
                m_command = value;
            }
        }

        /// <summary>
        /// Gets or sets a command parameter associated with command.
        /// </summary>
        /// <seealso cref="ICommand"/>
        public object CommandParameter
        {
            get
            {
                return m_commandParameter;
            }

            set
            {
                m_commandParameter = value;
            }
        }

        /// <summary>
        /// Gets the name of the QATCustomization dialog group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string GroupName
        {
            get
            {
                return (string)GetValue(GroupNameProperty);
            }

            internal set
            {
                SetValue(GroupNameProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the Label property.
        /// </summary>
        /// <value></value>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            internal set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the Image property.
        /// </summary>
        /// <value></value>
        public ImageSource SmallIcon
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }

            internal set
            {
                SetValue(ImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets tooltip of the control.
        /// </summary>
        public object ToolTip
        {
            get
            {
                return (object)GetValue(ToolTipProperty);
            }

            set
            {
                SetValue(ToolTipProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSynchronizedwithQAT
        {
            get;

            set;
        }

        internal bool IsItemsHost
        {
            get;
            set;
        }

        internal UIElement Host
        {
            get;
            set;
        }

        #region Dependency Properties
        /// <summary>
        /// Defines group name in which command will be placed.  This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(RibbonCommandProvider), new PropertyMetadata(null));

        /// <summary>
        /// Defines the text that will be displayed for control that use this command.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(RibbonCommandProvider), new PropertyMetadata(null));

        /// <summary>
        /// Defines the image that will be displayed for control that use this command.
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonCommandProvider), new PropertyMetadata(null));

        /// <summary>
        /// Defines the tooltip that will be displayed for control that use this command.
        /// </summary>
        public static readonly DependencyProperty ToolTipProperty =
            DependencyProperty.Register("ToolTip", typeof(object), typeof(RibbonCommandProvider), new PropertyMetadata(null));
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCommandProvider"/> class.
        /// </summary>
        public RibbonCommandProvider()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCommandProvider"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public RibbonCommandProvider(string label)
        {
            Label = label;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCommandProvider"/> class.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="label">The label param value.</param>
        /// <param name="image">The image param value.</param>
        /// <param name="toolTip">The tool tip value.</param>
        public RibbonCommandProvider(string groupName, string label, string image, object toolTip)
            : this(groupName, label, !string.IsNullOrEmpty(image) ? new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute)) : null, toolTip)
        {
            if (!Uri.IsWellFormedUriString(image, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Image represents bad path format", "image");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCommandProvider"/> class.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="label">The label value.</param>
        /// <param name="image">The image source.</param>
        /// <param name="toolTip">The tool tip value.</param>
        public RibbonCommandProvider(string groupName, string label, ImageSource image, object toolTip)
            : this(label)
        {
            GroupName = groupName;
            SmallIcon = image;
            ToolTip = toolTip;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonCommandProvider"/> class.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="label">The label Text that will be displayed for control that use this command.</param>
        /// <param name="image">The image that will be displayed for control that use this command.</param>
        public RibbonCommandProvider(string groupName, string label, string image)
            : this(groupName, label, !string.IsNullOrEmpty(image) ? new BitmapImage(new Uri(image, UriKind.RelativeOrAbsolute)) : null, null)
        {
            if (!Uri.IsWellFormedUriString(image, UriKind.RelativeOrAbsolute))
            {
                throw new ArgumentException("Image represents bad path format", "image");
            }
        }
        #endregion
    }
}
