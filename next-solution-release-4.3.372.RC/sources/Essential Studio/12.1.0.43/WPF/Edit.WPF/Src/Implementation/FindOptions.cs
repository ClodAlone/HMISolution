#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.ComponentModel;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(true)]
#endif
    /// <summary>
    ///
    /// </summary>
    public class FindOptions : DependencyObject, INotifyPropertyChanged
    {
        private string m_findText = String.Empty;

        private string m_replaceText = String.Empty;

        private bool m_isMatchCase = false;

        private bool m_isMatchWholeWord = false;

        private bool m_isSearchUp = false;

        private bool m_isIncludeHiddenText = false;

        private bool m_isSelectionSelected = false;

        private bool m_isWholeWordChecked = false;

        private bool m_isPrefix = false;

        private bool m_isSubstring = true;

        private EditControl m_editor;

        private NonStickingPopup m_parentWindow;

        private bool m_isFindActive;

        private bool m_isReplaceActive;

        private bool m_isFindSymbolActive;

        private int m_dropDownListSelectedIndex = 0;

        private FindAndReplaceCommand m_findNextCommand;

        private FindAndReplaceCommand m_replaceCommand;

        private FindAndReplaceCommand m_replaceAllCommand;

        private FindAndReplaceCommand m_findAllCommand;

        private FindAndReplaceCommand m_showFindTabCommand;

        private FindAndReplaceCommand m_showReplaceTabCommand;

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty ReplaceHistoryProperty = DependencyProperty.Register("ReplaceHistory",
            typeof(Stack<string>), typeof(FindOptions), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty FindHistoryProperty = DependencyProperty.Register("FindHistory",
            typeof(Stack<string>), typeof(FindOptions), new PropertyMetadata(null));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty StatusMessageProperty = DependencyProperty.Register("StatusMessage",
            typeof(string), typeof(FindOptions), new PropertyMetadata("Ready"));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsFindTabActiveProperty = DependencyProperty.Register("IsFindTabActive",
            typeof(bool), typeof(FindOptions), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsReplaceTabActiveProperty = DependencyProperty.Register("IsReplaceTabActive",
            typeof(bool), typeof(FindOptions), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsFindSymbolTabActiveProperty = DependencyProperty.Register("IsFindSymbolTabActive",
            typeof(bool), typeof(FindOptions), new PropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public ICommand FindNextCommand
        {
            get
            {
                if (m_findNextCommand == null)
                {
                    m_findNextCommand = new FindAndReplaceCommand(param => this.ExecuteFindCommand(), param => this.FindCanExecute());
                }
                return m_findNextCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICommand ReplaceCommand
        {
            get
            {
                if (m_replaceCommand == null)
                {
                    m_replaceCommand = new FindAndReplaceCommand(param => this.ExecuteReplaceCommand(), param => this.ReplaceCanExecute());
                }
                return m_replaceCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICommand ReplaceAllCommand
        {
            get
            {
                if (m_replaceAllCommand == null)
                {
                    m_replaceAllCommand = new FindAndReplaceCommand(param => this.ExecuteReplaceAllCommand(), param => this.ReplaceCanExecute());
                }
                return m_replaceAllCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICommand FindAllCommand
        {
            get
            {
                if (m_findAllCommand == null)
                {
                    m_findAllCommand = new FindAndReplaceCommand(param => this.ExecuteFindAllCommand(), param => this.FindCanExecute());
                }
                return m_findAllCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICommand ShowFindTabCommand
        {
            get
            {
                if (m_showFindTabCommand == null)
                {
                    m_showFindTabCommand = new FindAndReplaceCommand(param => this.ExecuteShowFindTabCommand());
                }
                return m_showFindTabCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ICommand ShowReplaceTabCommand
        {
            get
            {
                if (m_showReplaceTabCommand == null)
                {
                    m_showReplaceTabCommand = new FindAndReplaceCommand(param => this.ExecuteShowReplaceTabCommand());
                }
                return m_showReplaceTabCommand;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Stack<string> ReplaceHistory
        {
            get { return (Stack<string>)GetValue(ReplaceHistoryProperty); }
            set { SetValue(ReplaceHistoryProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public Stack<string> FindHistory
        {
            get { return (Stack<string>)GetValue(FindHistoryProperty); }
            set { SetValue(FindHistoryProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFindTabActive
        {
            get { return (bool)GetValue(IsFindTabActiveProperty); }
            set { SetValue(IsFindTabActiveProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsReplaceTabActive
        {
            get { return (bool)GetValue(IsReplaceTabActiveProperty); }
            set { SetValue(IsReplaceTabActiveProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFindSymbolTabActive
        {
            get { return (bool)GetValue(IsFindSymbolTabActiveProperty); }
            set { SetValue(IsFindSymbolTabActiveProperty, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public int DropDownListSelectedIndex
        {
            get
            {
                return m_dropDownListSelectedIndex;
            }

            set
            {
                m_dropDownListSelectedIndex = value;
                OnPropertyChanged("DropDownListSelectedIndex");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFindActive
        {
            get
            {
                return m_isFindActive;
            }

            set
            {
                m_isFindActive = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsReplaceActive
        {
            get
            {
                return m_isReplaceActive;
            }

            set
            {
                m_isReplaceActive = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsFindSymbolActive
        {
            get
            {
                return m_isFindSymbolActive;
            }

            set
            {
                m_isFindSymbolActive = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public EditControl Editor
        {
            get
            {
                return m_editor;
            }

            set
            {
                m_editor = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public NonStickingPopup ParentWindow
        {
            get
            {
                return m_parentWindow;
            }

            set
            {
                m_parentWindow = value;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string FindText
        {
            get
            {
                return m_findText;
            }

            set
            {
                m_findText = value;
                OnPropertyChanged("FindText");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public string ReplaceText
        {
            get
            {
                return m_replaceText;
            }

            set
            {
                m_replaceText = value;
                OnPropertyChanged("ReplaceText");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsMatchCase
        {
            get
            {
                return m_isMatchCase;
            }

            set
            {
                m_isMatchCase = value;
                OnPropertyChanged("IsMatchCase");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsMatchWholeWord
        {
            get
            {
                return m_isMatchWholeWord;
            }

            set
            {
                m_isMatchWholeWord = value;
                OnPropertyChanged("IsMatchWholeWord");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSearchUp
        {
            get
            {
                return m_isSearchUp;
            }

            set
            {
                m_isSearchUp = value;
                OnPropertyChanged("IsSearchUp");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsIncludeHiddenText
        {
            get
            {
                return m_isIncludeHiddenText;
            }

            set
            {
                m_isIncludeHiddenText = value;
                OnPropertyChanged("IsIncludeHiddenText");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsWholeWordChecked
        {
            get
            {
                return m_isWholeWordChecked;
            }

            set
            {
                m_isWholeWordChecked = value;
                OnPropertyChanged("IsWholeWordChecked");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsPrefix
        {
            get
            {
                return m_isPrefix;
            }

            set
            {
                m_isPrefix = value;
                OnPropertyChanged("IsPrefix");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSubstring
        {
            get
            {
                return m_isSubstring;
            }

            set
            {
                m_isSubstring = value;
                OnPropertyChanged("IsSubstring");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSelectionSelected
        {
            get
            {
                return m_isSelectionSelected;
            }

            set
            {
                if (m_isSelectionSelected != value)
                {
                    m_isSelectionSelected = value;
                    OnPropertyChanged("IsSelectionSelected");
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="editor"></param>
        /// <param name="parentwindow"></param>
        public FindOptions(EditControl editor, NonStickingPopup parentwindow)
        {
            this.m_editor = editor;
            this.m_parentWindow = parentwindow;

            this.FindHistory = new Stack<string>();
            this.ReplaceHistory = new Stack<string>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="editor"></param>
        public FindOptions(EditControl editor)
        {
            this.m_editor = editor;
            this.FindHistory = new Stack<string>();
            this.ReplaceHistory = new Stack<string>();
            this.IsSubstring = true;
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteFindCommand()
        {
            if (this.m_editor != null)
            {
                m_editor.ExecuteFindNext();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteReplaceCommand()
        {
            if (this.m_editor != null)
            {
                m_editor.ExecuteReplace();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteReplaceAllCommand()
        {
            if (this.m_editor != null)
            {
                m_editor.ExecuteReplaceAll();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteFindAllCommand()
        {
            if (this.m_editor != null)
            {
                m_editor.ExecuteFindAll();
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteShowFindTabCommand()
        {
            if (this.m_editor != null)
            {
                if (this.DropDownListSelectedIndex == 0)
                {
                    m_editor.SetActiveTab(Tabs.FindTab);
                }
                else
                {
                    m_editor.SetActiveTab(Tabs.FindSymbolTab);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void ExecuteShowReplaceTabCommand()
        {
            if (this.m_editor != null)
            {
                m_editor.SetActiveTab(Tabs.ReplaceTab);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private bool FindCanExecute()
        {
            if (this.m_editor != null)
            {
                return this.m_findText != String.Empty;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private bool ReplaceCanExecute()
        {
            if (this.m_editor != null)
            {
                return this.m_findText != String.Empty && !this.m_editor.IsReadOnly;
            }
            else
            {
                return false;
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        ///
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged Members

        /// <summary>
        ///
        /// </summary>
        public class FindAndReplaceCommand : ICommand
        {
            #region Fields

            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            #endregion Fields

            #region Constructors

            /// <summary>
            ///
            /// </summary>
            /// <param name="execute"></param>
            public FindAndReplaceCommand(Action<object> execute)
                : this(execute, null)
            {
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="execute"></param>
            /// <param name="canExecute"></param>
            public FindAndReplaceCommand(Action<object> execute, Predicate<object> canExecute)
            {
                if (execute == null)
                    throw new ArgumentNullException("execute");

                _execute = execute;
                _canExecute = canExecute;
            }

            #endregion Constructors

            #region ICommand Members

            /// <summary>
            ///
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute == null ? true : _canExecute(parameter);
            }

            /// <summary>
            ///
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="parameter"></param>
            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            #endregion ICommand Members
        }
    }
}