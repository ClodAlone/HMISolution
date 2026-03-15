// <copyright file="EditCommands.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Collections.Generic;
using System.Windows.Input;

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// EditCommands is a Static class that contains all definitions of RoutedUICommands
    /// that can be used in EditControl.
    /// </summary>
    /// <remarks>
    /// The EditCommand class contains the Edit menu properties like New, Open, Save,
    /// Copy, Cut, Expandall, Collapseall etc.
    /// </remarks>
    /// <seealso
    /// cref="Syncfusion.Windows.Edit.EditCommands">Syncfusion.Windows.Edit.EditCommands</seealso>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public static class EditCommands
    {
        #region RoutedUICommand Definitions

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for New operation
        /// </summary>
        public static RoutedUICommand New = new RoutedUICommand("New", "New", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Open operation
        /// </summary>
        public static RoutedUICommand Open = new RoutedUICommand("Open", "Open", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Save operation
        /// </summary>
        public static RoutedUICommand Save = new RoutedUICommand("Save", "Save", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Copy operation
        /// </summary>
        public static RoutedUICommand Copy = new RoutedUICommand("Copy", "Copy", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Cut operation
        /// </summary>
        public static RoutedUICommand Cut = new RoutedUICommand("Cut", "Cut", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Paste operation
        /// </summary>
        public static RoutedUICommand Paste = new RoutedUICommand("Paste", "Paste", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for SelectAll operation
        /// </summary>
        public static RoutedUICommand SelectAll = new RoutedUICommand("Select All", "SelectAll", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Undo operation
        /// </summary>
        public readonly static RoutedUICommand Undo = new RoutedUICommand("Undo", "Undo", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Redo operation
        /// </summary>
        public static RoutedUICommand Redo = new RoutedUICommand("Redo", "Redo", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Delete operation
        /// </summary>
        public static RoutedUICommand Delete = new RoutedUICommand("Delete", "Delete", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Backspace operation
        /// </summary>
        public static RoutedUICommand Backspace = new RoutedUICommand("Backspace", "Backspace", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for IgnoreKeys operation
        /// </summary>
        internal static RoutedUICommand IgnoreKeys = new RoutedUICommand("Ignore", "Ignore", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for ExpandAll operation
        /// </summary>
        public static RoutedUICommand ExpandAll = new RoutedUICommand("Expand All", "Expandall", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for CollapseAll operation
        /// </summary>
        public static RoutedUICommand CollapseAll = new RoutedUICommand("Collapse All", "CollapseAll", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Find operation
        /// </summary>
        public static RoutedUICommand Find = new RoutedUICommand("Quick Find", "QuickFind", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Replace operation
        /// </summary>
        public static RoutedUICommand Replace = new RoutedUICommand("Quick Replace", "QuickReplace", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Search Text operation
        /// </summary>
        public static RoutedUICommand Search = new RoutedUICommand("Search", "Search", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Search Text in selected text operation
        /// </summary>
        public static RoutedUICommand SearchInSelected = new RoutedUICommand("Search In Selected", "SearchSelected", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Find All references operation
        /// </summary>
        public static RoutedUICommand FindAllReferences = new RoutedUICommand("Find All References", "FindAll", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Show Intellisense operation
        /// </summary>
        public static RoutedUICommand ShowIntellisense = new RoutedUICommand("Show Intellisense", "ShowIntellisense", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Increase Indent operation
        /// </summary>
        public static RoutedUICommand IncreaseIndent = new RoutedUICommand("Increase Indent", "IncreaseIndent", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Decrease Indent operation
        /// </summary>
        public static RoutedUICommand DecreaseIndent = new RoutedUICommand("Decrease Indent", "DecreaseIndent", typeof(EditControl));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Comment Selection operation
        /// </summary>
        public readonly static RoutedUICommand CommentSelection = new RoutedUICommand("Comment Selection", "CommentSelection", typeof(EditCommands));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for UnComment Selection operation
        /// </summary>
        public readonly static RoutedUICommand UncommentSelection = new RoutedUICommand("Uncomment Selection", "UncommentSelection", typeof(EditCommands));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Inserting an empty line (Ctrl + Enter) operation.
        /// </summary>
        public readonly static RoutedUICommand InsertNewLine = new RoutedUICommand("Insert New Line", "InsertNewLine", typeof(EditCommands));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for selecting current word.
        /// </summary>
        internal readonly static RoutedUICommand SelectCurrentWord = new RoutedUICommand("Select Current Word", "SelectCurrentWord", typeof(EditCommands));

        /// <summary>
        /// Initializes a new instance of the RoutedUICommand class for Auto-indentation operation.
        /// </summary>
        public readonly static RoutedUICommand AutoIndent = new RoutedUICommand("Auto Indent", "AutoIndent", typeof(EditCommands));

        internal static void InitializeCommandKeyGestures()
        {
            New.InputGestures.Add(new KeyGestureExt(Key.N, ModifierKeys.Control));
            Open.InputGestures.Add(new KeyGestureExt(Key.O, ModifierKeys.Control));
            Save.InputGestures.Add(new KeyGestureExt(Key.S, ModifierKeys.Control));
            Copy.InputGestures.Add(new KeyGestureExt(Key.C, ModifierKeys.Control));
            Copy.InputGestures.Add(new KeyGestureExt(Key.C, ModifierKeys.Control | ModifierKeys.Shift));
            Copy.InputGestures.Add(new KeyGestureExt(Key.Insert, ModifierKeys.Control));
            Cut.InputGestures.Add(new KeyGestureExt(Key.X, ModifierKeys.Control));
            Cut.InputGestures.Add(new KeyGestureExt(Key.X, ModifierKeys.Control | ModifierKeys.Shift));
            Cut.InputGestures.Add(new KeyGestureExt(Key.Delete, ModifierKeys.Shift));
            Paste.InputGestures.Add(new KeyGestureExt(Key.V, ModifierKeys.Control));
            Paste.InputGestures.Add(new KeyGestureExt(Key.V, ModifierKeys.Control | ModifierKeys.Shift));
            Paste.InputGestures.Add(new KeyGestureExt(Key.Insert, ModifierKeys.Shift));
            SelectAll.InputGestures.Add(new KeyGestureExt(Key.A, ModifierKeys.Control));
            Undo.InputGestures.Add(new KeyGestureExt(Key.Z, ModifierKeys.Control));
            Redo.InputGestures.Add(new KeyGestureExt(Key.Z, ModifierKeys.Control | ModifierKeys.Shift));
            Redo.InputGestures.Add(new KeyGestureExt(Key.Y, ModifierKeys.Control));
            Redo.InputGestures.Add(new KeyGestureExt(Key.Y, ModifierKeys.Control | ModifierKeys.Shift));
            Delete.InputGestures.Add(new KeyGestureExt(Key.Delete));
            Delete.InputGestures.Add(new KeyGestureExt(Key.H, ModifierKeys.Control | ModifierKeys.Shift));
            Backspace.InputGestures.Add(new KeyGestureExt(Key.Back));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.A, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.B, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.D, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.E, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.I, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.J, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.K, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.L, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.M, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.N, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.O, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.P, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.Q, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.R, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.T, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.U, ModifierKeys.Control | ModifierKeys.Shift));
            IgnoreKeys.InputGestures.Add(new KeyGestureExt(Key.Y, ModifierKeys.Control | ModifierKeys.Shift));
            Find.InputGestures.Add(new KeyGestureExt(Key.F, ModifierKeys.Control));
            Replace.InputGestures.Add(new KeyGestureExt(Key.H, ModifierKeys.Control));
            Search.InputGestures.Add(new KeyGestureExt(Key.F3));
            SearchInSelected.InputGestures.Add(new KeyGestureExt(Key.F3, ModifierKeys.Control));
            FindAllReferences.InputGestures.Add(new KeyGestureExt(Key.F12, ModifierKeys.Shift));
            ShowIntellisense.InputGestures.Add(new KeyGestureExt(Key.Space, ModifierKeys.Control));
            ShowIntellisense.InputGestures.Add(new KeyGestureExt(Key.J, ModifierKeys.Control));
            InsertNewLine.InputGestures.Add(new KeyGestureExt(Key.Enter, ModifierKeys.Control));
            SelectCurrentWord.InputGestures.Add(new KeyGestureExt(Key.W, ModifierKeys.Control | ModifierKeys.Shift));
            CommentSelection.InputGestures.Add(new KeyGestureExt(new List<Key>(new Key[] { Key.K, Key.C }), ModifierKeys.Control));
            CommentSelection.InputGestures.Add(new KeyGestureExt(new List<Key>(new Key[] { Key.E, Key.C }), ModifierKeys.Control));
            UncommentSelection.InputGestures.Add(new KeyGestureExt(new List<Key>(new Key[] { Key.K, Key.U }), ModifierKeys.Control));
            AutoIndent.InputGestures.Add(new KeyGestureExt(new List<Key>(new Key[] { Key.K, Key.D }), ModifierKeys.Control));
        }

        #endregion RoutedUICommand Definitions
    }
}