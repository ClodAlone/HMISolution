// <copyright file="InternalCommandManager.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Tools.Controls;
using System.Linq;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the internal command manager
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class InternalCommandManager
    {
        #region Private members
        /// <summary>
        /// Represents the command list
        /// </summary>
        private List<CommandBearer> m_commandList;

        /// <summary>
        /// Represents the QAT items
        /// </summary>
        private ObservableCollection<QuickAccessToolBarItem> m_items;

        internal QuickAccessToolBar QuickAccessToolBar;

        /// <summary>
        /// Represents to cancel QAT items when click Cancel button
        /// </summary>
        internal  ObservableCollection<QuickAccessToolBarItem> cancel_items;

        internal Dictionary<int, QuickAccessToolBarItem> cancelindex_Items;

        internal List<QuickAccessToolBarItem> changeItems;

        internal  List<QuickAccessToolBarItem> duplicateItems;

        internal ObservableCollection<QuickAccessToolBarItem> ordered_Items;

        /// <summary>
        /// Represents the default items
        /// </summary>
        private List<UIElement> m_defaultItems;

        internal List<UIElement> m_defaultQATItems;

        internal List<UIElement> m_defaultAutoPersistItems;

        internal bool cancelInsert = false;

        internal bool canRemove = false;

        internal int cancelIndex = 0;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<QuickAccessToolBarItem> Items
        {
            get
            {
                return m_items as IEnumerable<QuickAccessToolBarItem>;
            }
        }

        /// <summary>
        /// current QAT item.
        /// </summary>
        private static CommandBearer currentItem;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalCommandManager"/> class.
        /// </summary>
        public InternalCommandManager()
        {
            m_commandList = new List<CommandBearer>();
            m_items = new ObservableCollection<QuickAccessToolBarItem>();
            cancel_items = new ObservableCollection<QuickAccessToolBarItem>();
            cancelindex_Items = new Dictionary<int, QuickAccessToolBarItem>();
            duplicateItems = new List<QuickAccessToolBarItem>();
            ordered_Items = new ObservableCollection<QuickAccessToolBarItem>();
            changeItems = new List<QuickAccessToolBarItem>();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void Add(QuickAccessToolBarItem item)
        {
            if (item != null)
            {
                if (CanAdd(item))
                {
                    if (item.Command != null)
                    {
                        m_commandList.Add(new CommandBearer(item.Command, item.CommandParameter));
                    }

                    if (item.SourceElement != null)
                    {
                        Ribbon.SetIsQATItem(item.SourceElement, true);
                    }

                    m_items.Add(item);
                    changeItems.Add(item);
                    duplicateItems.Add(item);                    
                }
                //else
                //{
                //    throw new ArgumentException("item already exists and can not be duplicated");
                //}
            }
            else
            {
                throw new ArgumentNullException("item", "item can not be null");
            }
        }

        internal void AddToCancel(QuickAccessToolBarItem item)
        {
            if (item != null)
            {
                cancel_items.Add(item);
            }
            else
            {
                throw new ArgumentNullException("item", "item can not be null");
            }
        }

        internal void AddItemsIndexToCancel(int selectedIndex, QuickAccessToolBarItem toolbarItem)
        {
            if (selectedIndex != -1)
            {
                if (cancelindex_Items.ContainsKey(selectedIndex))
                {
                    selectedIndex++;
                }
                //cancelindex_Items.Add(selectedIndex, toolbarItem);                
            }
            else
            {
                throw new ArgumentNullException("toolbarItem", "Item cannot be null");
            }

        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index param value.</param>
        /// <param name="item">The item value.</param>
        public void Insert(int index, QuickAccessToolBarItem item)
        {
            if (item != null)
            {
                if (CanAdd(item))
                {
                    if (item.Command != null)
                    {
                        m_commandList.Add(new CommandBearer(item.Command, item.CommandParameter));
                    }

                    if (item.SourceElement != null)
                    {
                        Ribbon.SetIsQATItem(item.SourceElement, true);
                    }

                    m_items.Insert(index, item);
                    changeItems.Insert(index,item);
                    duplicateItems.Insert(index,item);  
                    if (!cancelInsert)
                    {
                        if (cancel_items.Count == 0)
                        {
                            cancelIndex = 0;
                        }
                        cancel_items.Insert(cancelIndex, item);
                        cancelIndex++;
                    }
                }
                else
                {
                    throw new ArgumentException("item already exists and can not be duplicated");
                }
            }
            else
            {
                throw new ArgumentNullException("item", "item can not be null");
            }
        }

        /// <summary>
        /// Predicate method for checking command bearer in commands list.
        /// </summary>
        /// <param name="cmdBearer">The CMD bearer.</param>
        /// <returns>Return the bool value.</returns>
        //private static bool IsBearerContainsThisCommand(CommandBearer cmdBearer)
        //{
        //    if (cmdBearer.Command == CurrentItem.Command || (cmdBearer.CommandParameter != null && CurrentItem.CommandParameter != null && cmdBearer.Command == CurrentItem.Command && cmdBearer.CommandParameter == CurrentItem.CommandParameter))
        //    {                
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        private static bool IsBearerContainsThisCommand(CommandBearer cmdBearer)
        {
            if (cmdBearer.Command == null ^ CurrentItem.Command == null)
            {
                // One command is null, the other is not => not equal
                return false;
            }

            if (cmdBearer.Command == null && CurrentItem.Command == null)
            {
                // Both commands are null => not equal
                return false;
            }

            if (!cmdBearer.Command.Equals(CurrentItem.Command))
            {
                // Both commands are not equal => not equal
                return false;
            }

            //// Commands are equal, so we now compare the CommandParameter

            if (cmdBearer.CommandParameter == null ^ CurrentItem.CommandParameter == null)
            {
                // One CommandParameter is null, the other is not => not equal
                return false;
            }

            if (cmdBearer.CommandParameter == null && CurrentItem.CommandParameter == null)
            {
                // Both CommandParameters are null => equal
                return true;
            }

            // Both CommandParameters are not null.
            // Whether or not the two command bearers are equal depends on the equality of the CommandParameters.
            return cmdBearer.CommandParameter.Equals(CurrentItem.CommandParameter);
        }

        /// <summary>
        /// Gets the current item.
        /// </summary>
        /// <value>The current item.</value>
        private static CommandBearer CurrentItem
        {
            get
            {
                if (currentItem != null)
                {
                    return currentItem;
                }
                else
                {
                    currentItem = new CommandBearer();
                    return currentItem;
                }
            }
        }





        /// <summary>
        /// Removes the specified item to be removed.
        /// </summary>
        /// <param name="itemToBeRemoved">The item to be removed.</param>
        public void Remove(QuickAccessToolBarItem itemToBeRemoved)
        {
            if (itemToBeRemoved != null)
            {
                if (m_items.Contains(itemToBeRemoved))
                {
                    m_items.Remove(itemToBeRemoved);                          
                }
                if (changeItems.Contains(itemToBeRemoved))
                {
                    changeItems.Remove(itemToBeRemoved);                          
                }
                if (!canRemove)
                {
                    duplicateItems.Remove(itemToBeRemoved);
                }

                CurrentItem.Command = itemToBeRemoved.Command;
                CurrentItem.CommandParameter = itemToBeRemoved.CommandParameter;

                if (m_commandList.Contains(m_commandList.Find(IsBearerContainsThisCommand)))
                {
                    m_commandList.Remove(m_commandList.Find(IsBearerContainsThisCommand));
                }

                CurrentItem.Command = null;
                CurrentItem.CommandParameter = null;

            }
            else
            {
                Debug.WriteLine("InternalCommandManager does not contain such item");
            }
        }

        /// <summary>
        /// Removes the specified cloned element.
        /// </summary>
        /// <param name="clonedElement">The cloned element.</param>
        public void Remove(UIElement clonedElement)
        {
            QuickAccessToolBarItem itemToBeRemoved = GetItemByClone(clonedElement);
            Remove(itemToBeRemoved);
        }

        /// <summary>
        /// Gets the item by clone.
        /// </summary>
        /// <param name="clonedElement">The cloned element.</param>
        /// <returns>item by clone</returns>
        public QuickAccessToolBarItem GetItemByClone(UIElement clonedElement)
        {
            foreach (QuickAccessToolBarItem item in m_items)
            {
                if (item.ClonedElement == clonedElement)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether this instance can add the specified item.
        /// </summary>
        /// <param name="item">The item param value.</param>
        /// <returns>
        /// <c>true</c> if this instance can add the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanAdd(QuickAccessToolBarItem item)
        {
            CurrentItem.Command = item.Command;
            CurrentItem.CommandParameter = item.CommandParameter;

            if (item.Command != null && m_commandList.Contains(m_commandList.Find(IsBearerContainsThisCommand)))
            {
                return false;
            }

            CurrentItem.Command = null;
            CurrentItem.CommandParameter = null;

            if (item.SourceElement != null)
            {
                foreach (QuickAccessToolBarItem qatItem in m_items)
                {
                    if((qatItem.SourceElement.Equals(item.SourceElement)))
                        return false;
                }
                if (!Ribbon.GetIsQATItem(item.SourceElement))
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        /// <summary>
        /// Determines whether this instance can add the specified item.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// <c>true</c> if this instance can add the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanAdd(FrameworkElement element)
        {
            UIElement item = null;

            if (element != null)
            {
                item = element as UIElement;

                if (element.Parent is DropDownButton)
                {
                    if (!(element.Parent as DropDownButton).IsGroup)
                    {
                        item = element as UIElement;
                    }
                    else
                    {
                        FrameworkElement parent = element.Parent as FrameworkElement;
                        item = parent as UIElement;
                    }
                }
            }

            if (item is ICommandSource)
            {
                ICommand command = (item as ICommandSource).Command;
                object param = (item as ICommandSource).CommandParameter;

                CurrentItem.Command = command;
                CurrentItem.CommandParameter = param;

                if (command != null && m_commandList.Contains(m_commandList.Find(IsBearerContainsThisCommand)))
                {
                    return false;
                }

                CurrentItem.Command = null;
                CurrentItem.CommandParameter = null;
            }

            if (item != null)
            {
                foreach (QuickAccessToolBarItem qatItem in m_items)
                {
                    if (qatItem.SourceElement.Equals(item))
                        return false;
                }
                if (!Ribbon.GetIsQATItem(item))
                    return false;
                else
                    return true;
            }

            //This additional condition is added to avoid ApplicationMenuGroup to added in QAT.
            if (item is ApplicationMenuGroup)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            m_commandList.Clear();
           

            m_items.Clear();
            duplicateItems.Clear();
            changeItems.Clear();
            ordered_Items.Clear();
        }

        /// <summary>
        /// Gets the index of the element by.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>index of the element</returns>
        public QuickAccessToolBarItem GetElementByIndex(int index)
        {
            if ((index < 0) || (index >= m_items.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return m_items[index];
        }

        public bool ResetFlag = false;

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            if (m_defaultItems != null || m_defaultQATItems!=null)
            {
                Clear();
                InitializeDefaultItems(m_defaultQATItems);
                ResetFlag = true;
            }
            else
            {
                Clear();
                InitializeDefaultItems(m_defaultAutoPersistItems);
                ResetFlag = true;
            }
        }

        /// <summary>
        /// Initializes the default items.
        /// </summary>
        /// <param name="items">The items.</param>
        public void InitializeDefaultItems(List<UIElement> items)
        {
            m_defaultItems = items;
            if (items != null)
            {
                foreach (UIElement element in items)
                {
                    QuickAccessToolBarItem item = new QuickAccessToolBarItem(element);
                    Add(item);
                }
            }
        }
        #endregion

        /// <summary>
        /// Sorts the items collection by their index.
        /// </summary>
        public void SortByIndex()
        {
            var orderedList = this.Items.OrderBy(item => item.Index).ToList();

            this.Clear();

            foreach (QuickAccessToolBarItem item in orderedList)
                this.Add(item);
        }
    }

    /// <summary>
    /// Represents the CommandBearer
    /// </summary>
    internal class CommandBearer
    {
        #region Private members
        /// <summary>
        /// Command associated with provider.
        /// </summary>
        private ICommand m_command;

        /// <summary>
        /// CommandParameter associated with command.
        /// </summary>
        private object m_commandParameter;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBearer"/> class.
        /// </summary>
        internal CommandBearer()
        {
            this.m_command = null;
            this.m_commandParameter = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBearer"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameter">The command parameter.</param>
        internal CommandBearer(ICommand command, object commandParameter)
        {
            this.m_command = command;
            this.m_commandParameter = commandParameter;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a command associated with provider.
        /// </summary>
        /// <seealso cref="ICommand"/>
        protected internal ICommand Command
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
        protected internal object CommandParameter
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
        #endregion
    }
}
