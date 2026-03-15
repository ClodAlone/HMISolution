#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <typeparam name="TControl"></typeparam>
    public class ControlCommandBase<TBehavior, TControl>
        where TBehavior : CommandBehaviorBase<TControl>, new()
   where TControl : UIElement
    {
        private static readonly DependencyProperty CommandBehaviorProperty = DependencyProperty.RegisterAttached(
            "CommandBehavior",
            typeof(TBehavior),
            typeof(ControlCommandBase<TBehavior, TControl>),
            null);

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(ControlCommandBase<TBehavior, TControl>),
            new PropertyMetadata(OnSetCommandCallback));
        
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(ControlCommandBase<TBehavior, TControl>),
            new PropertyMetadata(OnSetCommandParameterCallback));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="command"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for selector")]
        public static void SetCommand(TControl selector, ICommand command)
        {
            selector.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for selector")]
        public static ICommand GetCommand(TControl selector)
        {
            return selector.GetValue(CommandProperty) as ICommand;
        }

        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var selector = dependencyObject as TControl;
            if (selector != null)
            {
                GetOrCreateBehavior(selector).Command = e.NewValue as ICommand;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="parameter"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for selector")]
        public static void SetCommandParameter(TControl selector, object parameter)
        {
            selector.SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for selector")]
        public static object GetCommandParameter(TControl selector)
        {
            return selector.GetValue(CommandParameterProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for selector")]
        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var selector = dependencyObject as TControl;
            if (selector != null)
            {
                GetOrCreateBehavior(selector).CommandParameter = e.NewValue;
            }
        }

        private static TBehavior GetOrCreateBehavior(TControl selector)
        {
            var behavior = selector.GetValue(CommandBehaviorProperty) as TBehavior;
            if (behavior == null)
            {
                behavior = new TBehavior();
                behavior.SetTargetObject(selector);
                selector.SetValue(CommandBehaviorProperty, behavior);
            }

            return behavior;
        }
    }
}
