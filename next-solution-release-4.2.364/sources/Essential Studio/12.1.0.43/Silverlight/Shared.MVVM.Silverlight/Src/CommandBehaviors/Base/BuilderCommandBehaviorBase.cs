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

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TControl"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <typeparam name="TReturn"></typeparam>
    public class BuilderCommandBehaviorBase<TControl, TEventArgs, TReturn> : CommandBehaviorBase<TControl> where TControl : Control
    {
        /// <summary>
        /// 
        /// </summary>
        protected Func<object, TEventArgs, TReturn> builder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public BuilderCommandBehaviorBase(Func<object, TEventArgs, TReturn> builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// 
        /// </summary>
        public BuilderCommandBehaviorBase()
            : this(null)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnEventRaised(object sender, TEventArgs e)
        {
            if (builder != null)
                SetCommandParameter(builder(sender, e));

            ExecuteCommand();
        }
    }
}
