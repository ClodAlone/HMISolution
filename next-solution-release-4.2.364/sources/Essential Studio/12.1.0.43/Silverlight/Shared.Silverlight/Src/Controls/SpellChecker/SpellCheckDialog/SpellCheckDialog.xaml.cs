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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpellCheckDialog : WindowControl
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SpellCheckCompleted;

        #region fields

        private int errorPointer = 0;
        internal ISpellEditor Editor;
        private int startIndexOffset = 0;
        private int errorCount = 0;
        private Dictionary<WordOccurrence, ObservableCollection<string>> rangeAndSuggestions = null;
        private bool previousReadOnly;
        private bool setReadOnly;
        private List<string> ignoredWords;
        private Dictionary<string, string> changeWord;
        private WordOccurrence CurrentWord;
        private int selectedIndex = 0;
        private string replaceWithText = string.Empty;

        #endregion

        #region ctor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checker"></param>
        /// <param name="editor"></param>
        public SpellCheckDialog(SpellChecker checker, ISpellEditor editor):base()
        {
            InitializeComponent();
            ignoredWords = new List<string>();
            changeWord = new Dictionary<string, string>();
            Editor = editor;
            Checker = checker;
            GetRangeAndSuggestions();
            UpdateSuggestion();
            SetReadOnly();
            this.Closing += new ClosedEventHandler(SpellCheckdialog_Closing);
            lstBox.Loaded += new RoutedEventHandler(lstBox_Loaded);
            lstBox.SelectionChanged += new SelectionChangedEventHandler(lstBox_SelectionChanged);
            richText.KeyDown += new KeyEventHandler(richText_KeyDown);
        }

        #endregion        

        #region public filds

        /// <summary>
        /// 
        /// </summary>
        public SpellChecker Checker
        {
            get;
            internal set;
        }

        internal bool IsReplaceEnabled
        {
            get
            {
                return replaceBtn.IsEnabled;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<string> Suggestions
        {
            get { return (ObservableCollection<string>)GetValue(SuggestionsProperty); }
            set { SetValue(SuggestionsProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register("Suggestions", typeof(ObservableCollection<string>), typeof(SpellCheckDialog), null);

        /// <summary>
        /// 
        /// </summary>
        public string NotinDictionary
        {
            get { return (string)GetValue(NotinDictionaryProperty); }
            set { SetValue(NotinDictionaryProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NotinDictionaryProperty =
            DependencyProperty.Register("NotinDictionary", typeof(string), typeof(SpellCheckDialog), new PropertyMetadata(string.Empty));

        #endregion

        #region private methods

        private void GetRangeAndSuggestions()
        {
            rangeAndSuggestions = Checker.SpellCheck(Editor.Text);
            errorCount = rangeAndSuggestions.Count;
        }

        private void UpdateSuggestion()
        {
            IterateIgnoredWordsAndReplaceWords();           
            Suggestions = GetCurrentSuggestion();
            NotinDictionary = GetCurrentWordAsString();
            UpdateSelectedIndex();
            UpdateSelection();
            IncrementErrorPointer();
        }

        private void IterateIgnoredWordsAndReplaceWords()
        {
            string currentWord = GetCurrentWordAsString();
            while (GetCurrentWordIgnored(currentWord) || GetCurrentWordHasToChange(currentWord))
            {
                IterateIgnoredWords();
                CheckCurrentWordToReplace();
                currentWord = GetCurrentWordAsString();
            }
        }

        private void IterateIgnoredWords()
        {
            string currentWord = GetCurrentWordAsString();
            while (GetCurrentWordIgnored(currentWord))
            {
                IncrementErrorPointer();
                currentWord = GetCurrentWordAsString();
            }
        }

        private void UpdateSelectedIndex()
        {
            if (Suggestions != null && Suggestions.Count > 0 && lstBox.Items.Count > 0)
                lstBox.SelectedIndex = 0;
        }

        private void CheckCurrentWordToReplace()
        {
            string currentWord = GetCurrentWordAsString();
            while (GetCurrentWordHasToChange(currentWord))
            {
                Replace(changeWord[currentWord], currentWord, false);
                IncrementErrorPointer();
                currentWord = GetCurrentWordAsString();
            }
        }

        private void IncrementErrorPointer()
        {
            if (errorCount >= errorPointer + 1)
                errorPointer++;
            else if (errorPointer >= errorCount)
            {
                if (Editor.HasMoreTextToCheck())
                {
                    Editor.UpdateTextField();
                    startIndexOffset = 0;
                    errorPointer = 0;
                    GetRangeAndSuggestions();
                    UpdateSuggestion();
                }
                else
                {
                    OnSpellCheckCompleted();
                }
            }
        }

        private void SetcurrentWordIgnored(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                ignoredWords.Add(word);
            }
        }

        private void SetCurrentWordHasToChange(string errorWord, string correctWord)
        {
            if (!string.IsNullOrEmpty(errorWord) && !string.IsNullOrEmpty(correctWord))
            {
                changeWord[errorWord] = correctWord;
            }
        }

        private bool GetCurrentWordHasToChange(string errorWord)
        {
            if (!string.IsNullOrEmpty(errorWord))
            {
                return changeWord.Keys.Contains(errorWord);
            }
            return false;
        }

        private bool GetCurrentWordIgnored(string word)
        {
            if (!string.IsNullOrEmpty(word))
            {
                return ignoredWords.Contains(word);
            }
            return false;
        }

        private void UpdateStartingIndexOffset(int originalLength, int newLength)
        {
            startIndexOffset = startIndexOffset + (newLength - originalLength);
        }
        
        private void UpdateSelection()
        {
            WordOccurrence word = GetCurrentWord();
            if (word != null)
            {
                Editor.Focus();
                Editor.Select(word.StartPosition + startIndexOffset, word.Word.Length);
                CurrentWord = new WordOccurrence(word.Word, word.StartPosition + startIndexOffset, word.StartPosition + startIndexOffset + word.Word.Length);
                UpdateRichText();
            }
        }

        private void UpdateRichText()
        {          
            string str = string.Empty;
            richText.Blocks.Clear();
            Paragraph paragraph = new Paragraph();
            richText.Blocks.Add(paragraph);
            if (CurrentWord.StartPosition != 0)
            {
                str = Editor.Text.Substring(0, CurrentWord.StartPosition);
                if (!string.IsNullOrEmpty(str))
                    paragraph.Inlines.Add(CreateSpan(str));
            }

            if (!string.IsNullOrEmpty(CurrentWord.Word))
                paragraph.Inlines.Add(CreateErrorSpan(CurrentWord.Word));

            str = Editor.Text.Substring(CurrentWord.EndPosition);

            if (!string.IsNullOrEmpty(str))
                paragraph.Inlines.Add(CreateSpan(str));

            BringLineToView(CurrentWord.EndPosition);
        }

        private void ChangeRichTextToNormal()
        {            
            if (richText.Blocks.Count > 0)
            {
                Paragraph para = richText.Blocks[0] as Paragraph;
                foreach (Inline inline in para.Inlines)
                {
                    if (inline is Span)
                    {
                        Color color = ((inline as Span).Foreground as SolidColorBrush).Color;
                        if (color == Colors.Red)
                        {
                            (inline as Span).Foreground = new SolidColorBrush(Colors.Black);
                            (inline as Span).FontWeight = FontWeights.Normal;
                        }
                    }
                }
            }
        }

        private void BringLineToView(int pos)
        {
            int i = 0;
            TextPointer pointer = richText.ContentStart;
            while (i <= pos)
            {
                pointer = pointer.GetNextInsertionPosition(LogicalDirection.Forward);
                i++;
            }

            richText.Selection.Select(pointer, pointer);
        }

        private ObservableCollection<string> GetCurrentSuggestion()
        {
            if (rangeAndSuggestions.Count > errorPointer)
            {
                ObservableCollection<string> col = rangeAndSuggestions.Values.ElementAt(errorPointer);
                if (col.Count > 0)
                {
                    selectedIndex = 0;
                    ReplaceEnabled(true);
                    return col;
                }
                else
                {
                    selectedIndex = -1;
                    col.Add("(No Suggestions Available)");
                    ReplaceEnabled(false);                  
                    return col;
                }
            }
            return null;
        }

        private WordOccurrence GetCurrentWord()
        {
            if (rangeAndSuggestions.Count > errorPointer)
                return rangeAndSuggestions.Keys.ElementAt(errorPointer);
            return null;
        }

        private string GetCurrentWordAsString()
        {
            WordOccurrence word = GetCurrentWord();
            if (word != null)
                return word.Word;
            return string.Empty;
        }

        internal void OnSpellCheckCompleted()
        {
            ResetReadOnly();
            if (SpellCheckCompleted != null)
                SpellCheckCompleted(this, EventArgs.Empty);
            Close();
        }       

        private void Replace(string word, string errorString, bool updateSuggestion)
        {
            int originalLength = errorString.Length;
            if (!updateSuggestion)
                Editor.Select(GetCurrentWord().StartPosition + startIndexOffset, GetCurrentWord().Word.Length);
            Editor.SelectedText = word;
            int newLength = word.Length;
            UpdateStartingIndexOffset(originalLength, newLength);
            if(updateSuggestion)
                UpdateSuggestion();
        }      

        private void RemoveCurrentWord()
        {
            string currentWord = GetCurrentWordAsString();

            for (int i = 0; i < rangeAndSuggestions.Count; i++)
            {
                if (rangeAndSuggestions.ElementAt(i).Key.Word == currentWord)
                {
                    rangeAndSuggestions.Remove(rangeAndSuggestions.ElementAt(i).Key);
                }
            }
        }

        private void SetReadOnly()
        {
            if (!setReadOnly && Editor != null)
            {
                if (Editor.ControlToCheck is TextBox || Editor.ControlToCheck is RichTextBox)
                {
                    previousReadOnly = Editor.ControlToCheck is TextBox ? (Editor.ControlToCheck as TextBox).IsReadOnly : (Editor.ControlToCheck as RichTextBox).IsReadOnly;
                    if (Editor.ControlToCheck is TextBox)
                        (Editor.ControlToCheck as TextBox).IsReadOnly = true;
                    else
                        (Editor.ControlToCheck as RichTextBox).IsReadOnly = true;
                    SetReadOnlyCalled();
                }
            }            
        }

        private Span CreateSpan(string text)
        {
            Span span = new Span();
            span.Inlines.Add(text);
            return span;
        }

        private Span CreateErrorSpan(string text)
        {
            Span span = CreateSpan(text);
            span.FontWeight = FontWeights.Bold;
            span.Foreground = new SolidColorBrush(Colors.Red);
            return span;
        }

        private void ResetReadOnly()
        {
            if (setReadOnly && Editor != null)
            {
                SetReadOnlyCalled();
                if (Editor.ControlToCheck is TextBox)
                    (Editor.ControlToCheck as TextBox).IsReadOnly = previousReadOnly;
                else if(Editor.ControlToCheck != null)
                    (Editor.ControlToCheck as RichTextBox).IsReadOnly = previousReadOnly;

                previousReadOnly = false;
            }
        }

        private void SetReadOnlyCalled()
        {
            setReadOnly = !setReadOnly;
        }

        private void ReplaceEnabled(bool value)
        {
            replaceBtn.IsEnabled = value;
            replaceAllBtn.IsEnabled = value;
            if (!value)
                replaceWithText = string.Empty;
        }

        #endregion

        #region event handlers

        void lstBox_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSelectedIndex();
        }

        void lstBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstBox.SelectedItem != null && selectedIndex==0)
                replaceWithText = lstBox.SelectedItem.ToString();
            else
                replaceWithText = string.Empty;
        }

        private void replaceWith_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(replaceWithText))
            {
                replaceBtn.IsEnabled = false;
                replaceAllBtn.IsEnabled = false;
            }
            else
            {
                replaceBtn.IsEnabled = true;
                replaceAllBtn.IsEnabled = true;
            }
        }

        void SpellCheckdialog_Closing(object sender, ClosedEventArgs e)
        {
            ResetReadOnly();
        }

        private void replace(object sender, RoutedEventArgs e)
        {
            if (ignore.Content.ToString() == "Ignore Once")
            {
                if (lstBox.SelectedItem != null)
                {
                    //Replace(lstBox.SelectedItem.ToString(), NotinDictionary, true);
                    Replace(replaceWithText, NotinDictionary, true);
                }
            }
            else
            {
                Editor.Select(0, Editor.Text.Length);
                richText.SelectAll();
                Editor.SelectedText = richText.Selection.Text;
                GetRangeAndSuggestions();
                startIndexOffset = 0;
                errorPointer = 0;
                UpdateSuggestion();
                ignore.Content = "Ignore Once";
                ignoreAllBtn.IsEnabled = true;
                addToDic.IsEnabled = true;
                replaceAllBtn.IsEnabled = true;
            }
        }

        private void replaceAll(object sender, RoutedEventArgs e)
        {
            if (lstBox.SelectedItem != null)
            {
                SetCurrentWordHasToChange(NotinDictionary, lstBox.SelectedItem.ToString());
                //Replace(lstBox.SelectedItem.ToString(), NotinDictionary, true);
                Replace(replaceWithText, NotinDictionary, true);
            }
        }

        private void ignoreAll(object sender, RoutedEventArgs e)
        {
            SetcurrentWordIgnored(NotinDictionary);
            UpdateSuggestion();
        }

        private void addToDictionary(object sender, RoutedEventArgs e)
        {
            Checker.AddWord(GetCurrentWordAsString());
            SetcurrentWordIgnored(NotinDictionary);
            UpdateSuggestion();
        }

        private void ignoreOnce(object sender, RoutedEventArgs e)
        {
            if (ignore.Content.ToString() == "Ignore Once")
            {
                UpdateSuggestion();
            }
            else
            {
                ignore.Content = "Ignore Once";
                ignoreAllBtn.IsEnabled = true;
                addToDic.IsEnabled = true;
                replaceAllBtn.IsEnabled = true;
                UpdateRichText();
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void richText_KeyDown(object sender, KeyEventArgs e)
        {
            ChangeRichTextToNormal();
            ignore.Content = "Undo Edit";
            replaceBtn.IsEnabled = true;
            replaceAllBtn.IsEnabled = false;
            ignoreAllBtn.IsEnabled = false;
            addToDic.IsEnabled = false;
        }

        #endregion
    }
}
