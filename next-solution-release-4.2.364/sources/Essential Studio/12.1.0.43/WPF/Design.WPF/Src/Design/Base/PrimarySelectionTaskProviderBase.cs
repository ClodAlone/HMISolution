// <copyright file="PrimarySelectionTaskProviderBase.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.Policies;

namespace Syncfusion.Windows.Design
{
    /// <summary>
    /// PrimarySelectionTaskProvider Base class helps us to provide the selection task provider for SmartTag designer.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PrimarySelectionTaskProviderBase : PrimarySelectionTaskProvider
    {
        #region Initialization

        /// <summary>
        /// Using to store delete task
        /// </summary>
        private Task deleteTask;

        /// <summary>
        /// UICommand for about task
        /// </summary>
        private static RoutedUICommand about = new RoutedUICommand();

        #endregion

        #region Implementation

        /// <summary>
        /// Initializes a new instance of the <see cref="PrimarySelectionTaskProviderBase"/> class.
        /// </summary>
        public PrimarySelectionTaskProviderBase()
        {
            deleteTask = new Task();
            deleteTask.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(Execute), new CanExecuteRoutedEventHandler(CanExecute)));
            Task cutTask = new Task();
            cutTask.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(Execute), new CanExecuteRoutedEventHandler(CanExecute)));
            base.Tasks.Add(deleteTask);
            base.Tasks.Add(cutTask);
        }
                
        /// <summary>
        /// Executes the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void Execute(object sender, ExecutedRoutedEventArgs args)
        {
            ModelItem selectedItem = this.Context.Items.GetValue<Selection>().PrimarySelection;
            ModelItem parent = selectedItem.Parent;

            if (parent != null)
            {
                if (parent.GetCurrentValue() is Panel)
                {
                    using (ModelEditingScope scope = parent.BeginEdit())
                    {
                        ModelItemCollection items3 = parent.Properties["Children"].Collection;
                        if (!items3.IsReadOnly)
                        {
                            items3.Remove(selectedItem);
                        }

                        scope.Complete();
                    }
                }
                else if (parent.GetCurrentValue() is ItemsControl)
                {
                    using (ModelEditingScope scope1 = parent.BeginEdit())
                    {
                        ModelItemCollection items3 = parent.Properties["Items"].Collection;
                        if (!items3.IsReadOnly)
                        {
                            items3.Remove(selectedItem);
                        }

                        scope1.Complete();
                    }
                }
                else if (parent.GetCurrentValue() is ContentControl)
                {
                    using (ModelEditingScope scope2 = parent.BeginEdit())
                    {
                        parent.Properties["Content"].ClearValue();
                        scope2.Complete();
                    }
                }
                else if (parent.GetCurrentValue() is Decorator)
                {
                    using (ModelEditingScope scope3 = parent.BeginEdit())
                    {
                        parent.Properties["Child"].ClearValue();
                        scope3.Complete();
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can execute the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void CanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = !PrimarySelectionAdornerProviderBase.IsPopUpActive;
            args.Handled = true;
        }

        #endregion
    }
}
