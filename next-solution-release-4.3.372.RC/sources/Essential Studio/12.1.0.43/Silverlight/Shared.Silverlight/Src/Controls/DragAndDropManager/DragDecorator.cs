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

namespace Syncfusion.Windows.Shared.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class DragDecorator : Control
    {
        /// <summary>
        /// 
        /// </summary>
        public DragDecorator()
        {
            DefaultStyleKey = typeof(DragDecorator);
        }

        private Border backBorder;

        private Path movePath;

        private Path copyPath;

        private Path impossiblePath;

        private TextBlock descriptionText;

        /// <summary>
        /// 
        /// </summary>
        public string DropDescription
        {
            get { return (string)GetValue(DropDescriptionProperty); }
            set { SetValue(DropDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropDescription.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DropDescriptionProperty =
            DependencyProperty.Register("DropDescription", typeof(string), typeof(DragDecorator), new PropertyMetadata(null));

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            backBorder = GetTemplateChild("PART_DragDescription") as Border;
            movePath = GetTemplateChild("MovePath") as Path;
            impossiblePath = GetTemplateChild("DragImpossiblePath") as Path;
            descriptionText = GetTemplateChild("DescriptionText") as TextBlock;
            copyPath = GetTemplateChild("CopyPath") as Path;
            base.OnApplyTemplate();
        }

        internal Border BackBorder
        {
            get
            {
                return backBorder;
            }
        }

        internal Path MovePath
        {
            get
            {
                return movePath;
            }
        }

        internal Path ImpossiblePath
        {
            get
            {
                return impossiblePath;
            }
        }

        internal TextBlock Descriptiontext
        {
            get
            {
                return descriptionText;
            }
        }

        internal Path CopyPath
        {
            get
            {
                return copyPath;
            }
        }
    }
}
