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

namespace Syncfusion.Windows.Shared.Olap
{
    /// <summary>
    /// Child member of the excluded member. It can be a child member of a child member.
    /// </summary>
    public class ChildMember
        : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildMember"/> class.
        /// </summary>
        public ChildMember()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the child members.
        /// </summary>
        /// <value>The child members.</value>
        public ChildMembers ChildMembers
        {
            get { return (ChildMembers)GetValue(ChildMembersProperty); }
            set { SetValue(ChildMembersProperty, value); }
        }

        #endregion

        #region Dependency Property

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(ChildMember), new UIPropertyMetadata(string.Empty));

        // Using a DependencyProperty as the backing store for ChildMembers.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildMembersProperty =
            DependencyProperty.Register("ChildMembers", typeof(ChildMembers), typeof(ChildMember), new UIPropertyMetadata(null));

        #endregion
    }
}
