// <copyright file="SelectionPointer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Edit
{
    /// <summary>
    /// SelectionPointer class is used to hold values related to text selection
    /// </summary>
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    public class SelectionPointer
    {
        #region Local Variables

        /// <summary>
        /// instance for StartIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int startindex;

        /// <summary>
        /// instance for EndIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int endindex;

        /// <summary>
        /// instance for StartLine property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int startline;

        /// <summary>
        /// instance for EndLine property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int endline;

        /// <summary>
        /// temp variable for LineIndex property.
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        private int temp;

        #endregion Local Variables

        #region Properties

        /// <summary>
        /// Gets or sets the Start line of the Selection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        /// <remarks>
        /// Specifies the StartLine.
        /// </remarks>
        public int StartLine
        {
            get
            {
                return startline;
            }

            set
            {
                temp = startline;
                startline = value;
                OnSelectionPointerChanged(new SelectionPointerChangedEventArgs("StartLine", temp, startline));
            }
        }

        /// <summary>
        /// Gets or sets the End line of the Selection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        /// <remarks>
        /// Specifies the EndLine.
        /// </remarks>
        public int EndLine
        {
            get
            {
                return endline;
            }

            set
            {
                temp = endline;
                endline = value;
                OnSelectionPointerChanged(new SelectionPointerChangedEventArgs("EndLine", temp, endline));
            }
        }

        /// <summary>
        /// Gets or sets the Start Index of the Selection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        /// <remarks>
        /// Specifies the StartIndex.
        /// </remarks>
        public int StartIndex
        {
            get
            {
                return startindex;
            }

            set
            {
                temp = startindex;
                startindex = value;
                OnSelectionPointerChanged(new SelectionPointerChangedEventArgs("StartIndex", temp, startindex));
            }
        }

        /// <summary>
        /// Gets or sets the End Index of the Selection
        /// </summary>
        /// <value>
        /// Type: System.Int32
        /// </value>
        /// <remarks>
        /// Specifies the EndIndex.
        /// </remarks>
        public int EndIndex
        {
            get
            {
                return endindex;
            }

            set
            {
                temp = endindex;
                endindex = value;
                OnSelectionPointerChanged(new SelectionPointerChangedEventArgs("EndIndex", temp, endindex));
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionPointer"/> class.
        /// </summary>
        public SelectionPointer()
        {
        }

        #endregion Initialization

        #region Events

        /// <summary>
        /// Occurs when SelectionPointerChanged in the SelectionPointer.
        /// </summary>
        public event SelectionPointerChangedEventHandler SelectionPointerChanged;

        /// <summary>
        /// When PropertyChanged event raised, that property is changed on a component.
        /// </summary>
        /// <param name="args">The arguments of Selection pointer values changed</param>
        internal void OnSelectionPointerChanged(SelectionPointerChangedEventArgs args)
        {
            if (SelectionPointerChanged != null)
            {
                SelectionPointerChanged(this, args);
            }
        }

        #endregion Events
    }
}