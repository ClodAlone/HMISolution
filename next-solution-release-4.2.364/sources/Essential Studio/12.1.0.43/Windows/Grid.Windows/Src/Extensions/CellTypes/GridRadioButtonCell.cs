//-------------------------------------------------------------------------------------------------
// <copyright file="GridRadioButtonCell.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Security;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a radio button cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridRadioButtonCellModel"/> can serve as model for several <see cref="GridRadioButtonCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridRadioButtonCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridRadioButtonCellModel : GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridRadioButtonCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridRadioButtonCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridRadioButtonCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridRadioButtonCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRadioButtonCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridRadioButtonCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements the renderer part of a radio button cell.
    /// </summary>
    /// <remarks>
    /// You set up radio buttons using an array of  
    ///  <see cref="GridRadioButtonInfo"/> objects. Each info object in this
    ///  array corresponds to a single radio button. The properties for each button
    ///  that you can set include GridRadioButtonInfo.Description, 
    ///  GridRadioButtonInfo.Alignment and GridRadioButtonInfo.Enabled. To get a 
    ///  cell to use a particular GridRadioButtonInfo[], you set the 
    ///  GridStyleInfo.ChoiceList to string collection that holds the descriptions for 
    ///  each button. If you want to disable a particular button, end the description
    ///  with /disabled. You are limited to a maximum of ten buttons. The alignment setting
    ///  is determined from the GridStyleInfo.TextAlign property.
    /// </remarks>
    public class GridRadioButtonCellRenderer : GridStaticCellRenderer
    {
        private int MAXBUTTONS = 10;
        internal int selectedButtonIndex;

        /// <summary>
        /// Initializes a new GridRadioButtonCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridRadioButtonCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            SupportsFocusControl = true;

            MAXBUTTONS = 10;

            for (int i = 0; i < MAXBUTTONS; i++)
            {
                AddButton(new GridRadioButton(this, i));
            }

            this.Grid.CellButtonClicked += new GridCellButtonClickedEventHandler(ButtonClicked);
        }

        private void ButtonClicked(object sender, GridCellButtonClickedEventArgs e)
        {
            GridRadioButton button = e.Button as GridRadioButton;
            if (button == null || !button.Enabled)
            {
                return;
            }

            if (Grid.CurrentCell.RowIndex != e.RowIndex || Grid.CurrentCell.ColIndex != e.ColIndex)
            {
                Grid.CurrentCell.MoveTo(e.RowIndex, e.ColIndex, GridSetCurrentCellOptions.NoSetFocus);
            }

            GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
            if (style.ReadOnly || !this.NotifyCurrentCellChanging())
            {
                return;
            }

            this.selectedButtonIndex = e.ButtonIndex;
            this.Grid.Model[e.RowIndex, e.ColIndex].CellValue = selectedButtonIndex;
            this.ResetControlValue();
            this.ResetControlText();

            this.NotifyCurrentCellChanged();
        }

        /// <override/> 
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            try
            {
                if (style.CellValue == null || GridUtil.IsEmpty(style.CellValue.ToString()))
                {
                    selectedButtonIndex = -1;
                }
                else
                {
                    selectedButtonIndex = Convert.ToInt32(style.CellValue);
                }
            }
            catch
            {
                selectedButtonIndex = -1;
            }
        }

        private GridRadioButtonInfo GetButtonInfo(int buttonNumber, GridStyleInfo style)
        {
            StringCollection sc = style.ChoiceList as StringCollection;
            if (sc != null)
            {
                string des = sc[buttonNumber];
                bool disabled = des.ToLower().EndsWith("/disabled");
                if (disabled)
                {
                    des = des.Substring(0, des.Length - 9);
                }

                return new GridRadioButtonInfo(des, !disabled);
            }

            return null;
        }

        private int GetNumberOfButtons(GridStyleInfo style)
        {
            StringCollection sc = style.ChoiceList as StringCollection;
            if (sc != null)
            {
                return sc.Count;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Overriden to allow buttons to be printed.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <returns>returns boolean value</returns>
        protected override bool OnQueryShowButtons(int rowIndex, int colIndex, GridStyleInfo style)
        {
            return
                style.ShowButtons == GridShowButtons.Show
                || (style.ShowButtons == GridShowButtons.ShowCurrentRow && Grid.IsShowCurrentRow(rowIndex))
                || (style.ShowButtons == GridShowButtons.ShowCurrentCell && Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                || (style.ShowButtons == GridShowButtons.ShowCurrentCellEditing && Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex) && Grid.CurrentCell.IsEditing);
        }

        /// <override/>
        /// <summary>
        /// Draw the contents of specified cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="cellRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(System.Drawing.Graphics g, System.Drawing.Rectangle cellRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            base.Draw(g, cellRectangle, rowIndex, colIndex, style);

            int buttonWidth = GridRadioButton.RADIOBUTTONWIDTH;
            int numberButtons = GetNumberOfButtons(style);
            StringCollection buttonInfos = style.ChoiceList;

            Font f = style.Font.GdipFont;
            
            for (int i = 0; i < numberButtons; i++)
            {
                GridCellButton button = this.GetButton(i);
                string displayText = string.Empty;
                bool drawDisabled = false;
                Color c = style.TextColor;
                if (buttonInfos != null)
                {
                    GridRadioButtonInfo info = this.GetButtonInfo(i, style);
                    displayText = info.Description;
                    if (!info.Enabled)
                    {
                        c = Color.DimGray;
                    }
                }

                Rectangle rect = button.Bounds;
                Size lineSize = g.MeasureString(displayText, f, PointF.Empty, StringFormat.GenericDefault).ToSize();
                rect.Width = rect.Width - buttonWidth - 1; ////textrect minus buttonwidth...

                if (rect.Width < 0)
                {
                    return;
                }

                if (style.TextAlign == GridTextAlign.Left) 
                {
                    ////buttons on left
                    if (lineSize.Width < rect.Width)
                    {
                        rect.X = rect.X + rect.Width - lineSize.Width - 1;
                        rect.Width = lineSize.Width + 1;
                    }
                }
                else  
                {
                    ////buttons on right
                    rect.X = rect.X + buttonWidth + 1;
                }

                ////center vertically
                if (lineSize.Height < rect.Height)
                {
                    rect.Y = rect.Y + ((rect.Height - lineSize.Height) / 2);
                }

                ////style.TextAlign = GridTextAlign.Default;
                DrawText(g, displayText, f, rect, style, c, drawDisabled);
            }
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        /// True if changes were saved successfully; False if no changes were saved.
        /// </returns>
        /// <override/>
        protected override bool OnSaveChanges()
        {
            return true;
        }

        /// <summary>
        /// This method is called from PerformLayout to calculate the client rectangle given
        /// the inner rectangle of a cell and any boundaries of cell buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="innerBounds">The <see cref="System.Drawing.Rectangle"/> with the inner bounds of a cell.</param>
        /// <param name="buttonsBounds">An array of <see cref="System.Drawing.Rectangle"/> with bounds for each cell button element.</param>
        /// <returns>
        /// A <see cref="System.Drawing.Rectangle"/> with the bounds.
        /// </returns>
        /// <override/>
        protected override System.Drawing.Rectangle OnLayout(int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style, System.Drawing.Rectangle innerBounds, System.Drawing.Rectangle[] buttonsBounds)
        {
            Rectangle clientRectangle = GridMargins.RemoveMargins(innerBounds, style.TextMargins.ToMargins());
            StringCollection buttonInfos = style.ChoiceList;
            int count = GetNumberOfButtons(style);
            if (buttonInfos != null && count > 0)
            {
                if (style.RadioButtonAlignment == ButtonAlignment.Vertical)
                {
                    int width = clientRectangle.Width;
                    int height = clientRectangle.Height / count;
                    if (height == 0)
                    {
                        height = clientRectangle.Height;
                    }
                    GridVerticalAlignment verticalAlign = style.VerticalAlignment;
                    int yOffset = clientRectangle.Top;
                    if (verticalAlign == GridVerticalAlignment.Middle)
                    {
                        yOffset += (clientRectangle.Height - height) / 2;
                    }
                    else if (verticalAlign == GridVerticalAlignment.Bottom)
                    {
                        yOffset += (clientRectangle.Height - height);
                    }
                    int xOffset = clientRectangle.Left;
                    for (int n = 0; n < count; n++)
                    {
                        Rectangle bounds = new Rectangle(xOffset, yOffset, width, height);
                        buttonsBounds[n] = bounds;
                        if (buttonInfos != null)
                        {
                            ((GridRadioButton)GetButton(n)).Enabled = this.GetButtonInfo(n, style).Enabled;
                        }
                        yOffset += height;
                    }
                    
                }
                else
                {
                    int width = clientRectangle.Width / count;
                    ////clientRectangle.Width -= width;
                    int height = clientRectangle.Height;
                    if (height == 0)
                    {
                        height = clientRectangle.Height;
                    }
                    GridVerticalAlignment verticalAlign = style.VerticalAlignment;
                    int yOffset = clientRectangle.Top;
                    if (verticalAlign == GridVerticalAlignment.Middle)
                    {
                        yOffset += (clientRectangle.Height - height) / 2;
                    }
                    else if (verticalAlign == GridVerticalAlignment.Bottom)
                    {
                        yOffset += clientRectangle.Height - height;
                    }

                    int xOffset = clientRectangle.Left;

                    for (int n = 0; n < count; n++)
                    {
                        Rectangle bounds = new Rectangle(xOffset, yOffset, width, height);
                        buttonsBounds[n] = bounds;
                        if (buttonInfos != null)
                        {
                            ((GridRadioButton)GetButton(n)).Enabled = this.GetButtonInfo(n, style).Enabled;
                        }

                        xOffset += width;
                    }
                }
            }

            clientRectangle.Width = 0;
            return clientRectangle;
        }

        /// <override/>
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            GridRadioButton button = null;

            if (e.KeyCode == Keys.Right)
            {
                GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                selectedButtonIndex++;
                if (selectedButtonIndex >= GetNumberOfButtons(style))
                {
                    selectedButtonIndex = 0;
                }

                button = (GridRadioButton)this.GetButton(selectedButtonIndex);
            }
            else if (e.KeyCode == Keys.Left)
            {
                selectedButtonIndex--;
                if (selectedButtonIndex >= 0)
                {
                    button = (GridRadioButton)this.GetButton(selectedButtonIndex);
                }

                if (selectedButtonIndex < 0 || button == null)
                {
                    GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                    selectedButtonIndex = GetNumberOfButtons(style) - 1;
                    button = (GridRadioButton)this.GetButton(selectedButtonIndex);
                }
            }

            if (button != null)
            {
                if (!button.Enabled)
                {
                    OnKeyDown(e);
                }
                else
                {
                    GridStyleInfo style = Grid.Model[this.RowIndex, this.ColIndex];
                    if (!style.ReadOnly && this.NotifyCurrentCellChanging())
                    {
                        ////this.selectedButtonIndex = selectedButtonIndex;
                        this.Grid.Model[this.RowIndex, this.ColIndex].CellValue = selectedButtonIndex;

                        this.NotifyCurrentCellChanged();
                        e.Handled = true;
                    }
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        /// <override/>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            base.OnInitialize(rowIndex, colIndex);
            this.Grid.CurrentCell.BeginEdit();
        }
 }

    /// <summary>
    /// Defines the cell radio button of a <see cref="GridRadioButtonCellRenderer"/>.
    /// </summary>
    /// <remarks>
    /// This button is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True
    /// and the GridStyleInfo.ThemesEnabled is True for the cell.
    /// </remarks>
    public class GridRadioButton : GridCellButton
    {
        private int _index;
        private bool _enabled;

        ////private ThemedRadioButtonDrawing themedDrawing = null;

        /// <summary> Width of the radio button.
        /// </summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        static public int RADIOBUTTONWIDTH = 15;

        /// <summary>
        /// Initializes a new GridRadioButton.
        /// </summary>
        /// <param name="renderer">The GridRadioButtonCellRenderer object which owns this button.</param>
        /// <param name="index">The button's index in the collection of radio buttons for this cell.</param>
        public GridRadioButton(GridCellRendererBase renderer, int index)
            : base(renderer)
        {
            _index = index;
            _enabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////                if (this.themedDrawing != null)
                ////                {
                ////                    this.themedDrawing.Dispose();
                ////                    this.themedDrawing = null;
                ////                }
            }

            base.Dispose(disposing);
        }

        /// <override/>
        /// <summary>
        /// Draws a button using <see cref="ControlPaint.DrawButton(System.Drawing.Graphics,System.Drawing.Rectangle,System.Windows.Forms.ButtonState)"/> or if XP Themes
        /// are enabled, button will be drawn themed.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rect">The <see cref="System.Drawing.Rectangle"/> with the bounds.</param>
        /// <param name="buttonState">A <see cref="ButtonState"/> that specifies the current state.</param>
        /// <param name="style">The style information for the cell.</param>
        public override void DrawButton(System.Drawing.Graphics g, System.Drawing.Rectangle rect, System.Windows.Forms.ButtonState buttonState, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            GridRadioButtonCellRenderer renderer = (GridRadioButtonCellRenderer)this.Owner;
            if (renderer != null && renderer.selectedButtonIndex == _index)
            {
                buttonState |= ButtonState.Checked;
            }

            bool mixedStateSet = false;
            if ((buttonState & ButtonState.Inactive) > 0
                && (buttonState & ButtonState.Checked) > 0)
            {
                mixedStateSet = true;
            }

            if (mixedStateSet && style.Enabled)
            {
                buttonState &= ~ButtonState.Inactive;
                buttonState &= ~ButtonState.Checked;
            }

            if ((buttonState & ButtonState.Flat) > 0)
            {
                buttonState &= ~ButtonState.Flat;
                buttonState |= ButtonState.Normal;
            }

            if (!this.Enabled)
            {
                buttonState |= ButtonState.Inactive;
            }

            int width = GridRadioButton.RADIOBUTTONWIDTH;
            int X = rect.X + Math.Max(0, rect.Width - width);
            int Y = rect.Y;
            int height = rect.Height;

            if (style.TextAlign == GridTextAlign.Right)
            {
                X = rect.X;
            }
            
            if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                || (style.Themed && this.Grid.ThemesEnabled && this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))
            {
                rect = new Rectangle(X, Y, width, height);

                if ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                    && (this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.Office2003))
                {
                    rect.Y = rect.Y + Math.Max(0, (rect.Height / 2) - 6);
                }

                ////                if(this.themedDrawing == null)
                ////                    this.themedDrawing = new ThemedRadioButtonDrawing();
                ////                this.themedDrawing.DrawRadio(g, rect, buttonState, mixedStateSet);

                this.Grid.Model.Options.GridVisualStylesDrawing.DrawRadioStyle(g, rect, buttonState);
            }
            else
            {
                ControlPaint.DrawRadioButton(g, X, Y, width, height, buttonState);
            }
        }
    }

    /// <summary>
    /// Holds the state information regarding a radio button within a RadioButton cell.
    /// </summary>
    [Serializable]
    public class GridRadioButtonInfo : ISerializable, ICloneable
    {
        private string _description;
        private bool _enabled;

        /// <summary>
        /// Initializes a new <see cref="GridRadioButtonInfo"/> with default values.
        /// </summary>
        public GridRadioButtonInfo()
        {
            _enabled = true;
            _description = string.Empty;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRadioButtonInfo"/>.
        /// </summary>
        /// <param name="desc">The <see cref="GridRadioButtonInfo.Description"/> property value.</param>
        /// <param name="enabled">The <see cref="GridRadioButtonInfo.Enabled"/> property value.</param>
        public GridRadioButtonInfo(string desc, bool enabled)
        {
            this._enabled = enabled;
            this._description = desc;
        }

        /// <summary>
        /// Initializes a new <see cref="GridRadioButtonInfo"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridRadioButtonInfo(SerializationInfo info, StreamingContext context)
        {
            this._description = (string)info.GetValue("Description", typeof(string));
            this._enabled = (bool)info.GetValue("Enabled", typeof(bool));
        }

        /// <summary>
        /// Gets or sets the text displayed with the button.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        #region Implementation of ISerializable
        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the object.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("Description", this._description);
            info.AddValue("Enabled", this._enabled);
        }
        #endregion

        #region Implementation of ICloneable
        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A copy of this object.</returns>
        [DebuggerStepThrough()]
        public object Clone()
        {
            return new GridRadioButtonInfo(this.Description, this.Enabled);
        }
        #endregion
    }
}

