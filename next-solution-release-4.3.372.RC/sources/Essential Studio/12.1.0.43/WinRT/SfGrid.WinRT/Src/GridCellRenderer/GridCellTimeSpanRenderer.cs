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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Syncfusion.UI.Xaml.Grid.Utility;
using Syncfusion.UI.Xaml.ScrollAxis;
#if WPF
using Syncfusion.Windows.Shared;
#else
using Syncfusion.Windows.Tools.Controls;
#endif

namespace Syncfusion.UI.Xaml.Grid.Cells
{
    [ClassReference(IsReviewed = false)]
    public class GridCellTimeSpanRenderer : GridVirtualizingCellRenderer<TextBlock, TimeSpanEdit>
    {


        #region Display/Edit Binding Overrides
        public override void OnInitializeDisplayElement(RowColumnIndex rowColumnIndex, TextBlock uiElement, GridColumn column, object dataContext)
        {
            base.OnInitializeDisplayElement(rowColumnIndex, uiElement, column, dataContext);
            uiElement.Padding = new Thickness(4, 2, 0,0);
        }

        /// <summary>
        /// Called when [initialize edit element].
        /// </summary>
        /// <param name="rowColumnIndex">Index of the row column.</param>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">The column.</param>
        /// <param name="dataContext">The data context.</param>
        public override void OnInitializeEditElement(RowColumnIndex rowColumnIndex, TimeSpanEdit uiElement, GridColumn column, object dataContext)
        {
            InitializeEditUIElement(uiElement, column);
            BindingExpression = uiElement.GetBindingExpression(TimeSpanEdit.ValueProperty);
            var textAlignment = new Binding { Path = new PropertyPath("TextAlignment"), Mode = BindingMode.OneWay, Source = column, Converter = new TextAlignmentToHorizontalAlignmentConverter() };
            uiElement.SetBinding(Control.HorizontalContentAlignmentProperty, textAlignment);
            uiElement.VerticalAlignment = VerticalAlignment.Stretch;
        }
        #endregion

        #region Display/Edit Value Overrides

        /// <summary>
        /// Gets the control value.
        /// </summary>
        /// <returns></returns>
        public override object GetControlValue()
        {
            if (HasCurrentCellState)
                return CurrentCellRendererElement.GetValue(IsInEditing ? TimeSpanEdit.ValueProperty : TextBlock.TextProperty);
            return base.GetControlValue();
        }

        /// <summary>
        /// Sets the control value.
        /// </summary>
        /// <param name="value">The value.</param>
        public override void SetControlValue(object value)
        {
            if (!HasCurrentCellState) return;
            if (IsInEditing)
                ((TimeSpanEdit) CurrentCellRendererElement).Value = (TimeSpan?) value;
            else
                throw new Exception("Value cannot be Set for Unloaded Editor");
        }

        #endregion

        #region Wire/UnWire UIElements Overrides
        /// <summary>
        /// Called when [edit element loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        protected override void OnEditElementLoaded(object sender, RoutedEventArgs e)
        {            
            var uiElement = ((TimeSpanEdit)sender);
            uiElement.ValueChanged += OnValueChanged;
            if (this.HasCurrentCellState)
            {
                uiElement.Focus();
                if ((this.DataGrid.EditorSelectionBehavior == EditorSelectionBehavior.SelectAll) && PreviewInputText == null)
                {
                    uiElement.SelectAll();
                }
                else
                {
                    if (PreviewInputText == null)
                    {
                        var index = uiElement.Text.Length;
                        uiElement.Select(index + 1, 0);
                        return;
                    }
                    uiElement.Text += PreviewInputText;               
                    var careAtIndex = uiElement.Text.LastIndexOf(':');                    
                    uiElement.Select(careAtIndex + 1, uiElement.Text.Length - careAtIndex);
                    
                }
                PreviewInputText = null;  
            }
            base.OnEditElementLoaded(sender, e);
        }

        /// <summary>
        /// Called when [unwire edit UI element].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        protected override void OnUnwireEditUIElement(TimeSpanEdit uiElement)
        {
            uiElement.ValueChanged -= OnValueChanged;
        }
        #endregion

        #region ShouldGridTryToHandleKeyDown
        /// <summary>
        /// Let Renderer decide whether the parent grid should be allowed to handle keys and prevent
        /// the key event from being handled by the visual UIElement for this renderer. If this method
        /// returns true the parent grid will handle arrow keys and set the Handled flag in the event
        /// data. Keys that the grid does not handle will be ignored and be routed to the UIElement
        /// for this renderer.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs" /> object.</param>
        /// <returns>
        /// True if the parent grid should be allowed to handle keys; false otherwise.
        /// </returns>
        protected override bool ShouldGridTryToHandleKeyDown(KeyEventArgs e)
        {
            if (!HasCurrentCellState || !IsInEditing)
                return true;
            var CurrentCellUIElement = (TimeSpanEdit)CurrentCellRendererElement;
#if WPF                    
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    return false;
                case Key.Escape:
                {
#if WPF
                    if (CurrentCellUIElement != null)
                        BindingOperations.ClearBinding(CurrentCellUIElement, TimeSpanEdit.ValueProperty);
#endif                                        
                    return true;
                }
#if WPF
                case Key.Left:
                    return (CurrentCellUIElement.CaretIndex <= 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Right:                    
                    return ((CurrentCellUIElement.CaretIndex + CurrentCellUIElement.SelectionLength) >= CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.Home:
                    return (CurrentCellUIElement.CaretIndex == 0 && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
                case Key.End:
                    return (CurrentCellUIElement.CaretIndex == CurrentCellUIElement.Text.Length && !CheckControlKeyPressed() && !CheckShiftKeyPressed());
#endif
#if SILVERLIGHT
                case Key.Left:
                case Key.Right:
                return true;
#endif
            }            
#endif
            return base.ShouldGridTryToHandleKeyDown(e);
        }
        #endregion

        #region Static Helper Method
        internal static string DisplayText(TimeSpan Value, string Format)
        {
            var isDaysVisibile = false;
            var isHoursVisible = false;
            var isMinutesVisible = false;
            var isSecondsVisible = false;
            var Text = string.Empty;
            var tPosition = new Dictionary<int, char>();
            var tLength = new Dictionary<int, int>();
            var tStart = new Dictionary<int, int>();
            if (Value != null && Format != string.Empty)
            {
                var tspan = Value;
                var format = Format;
                var displayText = string.Empty;
                tPosition.Clear();
                tLength.Clear();
                tStart.Clear();

                if (Format != string.Empty)
                {

                    var literals = format.Split('\'');
                    bool isLiteral = false;
                    bool isStringRepeated = false;
                    for (int i = 0; i < Format.Length; i++)
                    {
                        char ch = format[i];
                        if (ch == '\'')
                        {
                            isLiteral = !isLiteral;
                            continue;
                        }

                        if (isLiteral && ch != '\'')
                        {
                            displayText += ch;
                            continue;
                        }
                        if (i + 1 < Format.Length)
                        {
                            if (format[i + 1] == format[i])
                            {
                                i++;
                                isStringRepeated = true;
                            }
                        }
                        if (ch == 'd')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Days.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'd');
                                    tLength.Add(displayText.Length + k, tspan.Days.ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isDaysVisibile = true;
                                }
                            }
                            catch { }
                            displayText += tspan.Days.ToString();
                            continue;
                        }
                        if (ch == 'h')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Hours.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'h');
                                    tLength.Add(displayText.Length + k, isDaysVisibile
                                            ? tspan.Hours.ToString().Length
                                            : ((int) tspan.TotalHours).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isHoursVisible = true;
                                }
                            }
                            catch { }
                            if (isDaysVisibile)
                            {
                                if (isStringRepeated)
                                {
#if !SyncfusionFramework3_5
                                    displayText += tspan.ToString("hh");
#else
                                    if (tspan.Hours.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Hours.ToString();
                                    else
                                        displayText += tspan.Hours.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Hours.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalHours).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 'm')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Minutes.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'm');
                                    if (isHoursVisible)
                                        tLength.Add(displayText.Length + k, tspan.Minutes.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalMinutes).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isMinutesVisible = true;
                                }
                            }
                            catch { }
                            if (isHoursVisible)
                            {
                                if (isStringRepeated)
                                {
#if !SyncfusionFramework3_5
                                    displayText += tspan.ToString("mm");
#else
                                   if(tspan.Minutes.ToString().Length<2)
                                       displayText=displayText+0+tspan.Minutes.ToString(); 
                                   else
                                       displayText += tspan.Minutes.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Minutes.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalMinutes).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 's')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Seconds.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 's');
                                    if (isMinutesVisible)
                                        tLength.Add(displayText.Length + k, tspan.Seconds.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalSeconds).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);
                                    isSecondsVisible = true;
                                }
                            }
                            catch { }
                            if (isMinutesVisible)
                            {
                                if (isStringRepeated)
                                {

#if !SyncfusionFramework3_5
                                    displayText += tspan.ToString("ss");
#else
                                    if (tspan.Seconds.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Seconds.ToString();
                                    else
                                        displayText += tspan.Seconds.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Seconds.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalSeconds).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        if (ch == 'z')
                        {
                            try
                            {
                                for (int k = 0; k < tspan.Milliseconds.ToString().Length + 1; k++)
                                {
                                    tPosition.Add(displayText.Length + k, 'z');
                                    if (isSecondsVisible)
                                        tLength.Add(displayText.Length + k, tspan.Milliseconds.ToString().Length);
                                    else
                                        tLength.Add(displayText.Length + k, ((int)tspan.TotalMilliseconds).ToString().Length);
                                    tStart.Add(displayText.Length + k, displayText.Length);

                                }
                            }
                            catch { }
                            if (isSecondsVisible)
                            {
                                if (isStringRepeated)
                                {

#if !SyncfusionFramework3_5
                                    displayText += tspan.ToString("fffffff");
#else
                                    if (tspan.Milliseconds.ToString().Length < 2)
                                        displayText = displayText + 0 + tspan.Milliseconds.ToString();
                                    else
                                        displayText += tspan.Milliseconds.ToString();
#endif
                                }
                                else
                                    displayText += tspan.Milliseconds.ToString();
                            }
                            else
                                displayText += ((int)tspan.TotalMilliseconds).ToString();
                            isStringRepeated = false;
                            continue;
                        }
                        displayText += ch;
                    }
                }
                Text = displayText;
            }
            return Text;
        }
        #endregion
        
        #region Event Handlers
        /// <summary>
        /// Called when [text changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            base.CurrentRendererValueChanged();
        }
        #endregion

        #region Private Members

        /// <summary>
        /// Processes the edit binding.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="column">Grid Column of the UI Element</param>
        private void InitializeEditUIElement(TimeSpanEdit uiElement, GridColumn column)
        {
            var timeSpanColumn = (GridTimeSpanColumn) column;
            var bind = timeSpanColumn.ValueBinding.CreateEditBinding(timeSpanColumn.UpdateTrigger);
            uiElement.SetBinding(TimeSpanEdit.ValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("AllowNull"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.AllowNullProperty, bind);
            bind = new Binding { Path = new PropertyPath("Format"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.FormatProperty, bind);
            bind = new Binding { Path = new PropertyPath("NullText"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.NullStringProperty, bind);
            bind = new Binding { Path = new PropertyPath("MaxValue"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.MaxValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("MinValue"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.MinValueProperty, bind);
            bind = new Binding { Path = new PropertyPath("ShowArrowButtons"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.ShowArrowButtonsProperty, bind);
#if WPF
            bind = new Binding { Path = new PropertyPath("TextDecorations"), Mode = BindingMode.TwoWay, Source = timeSpanColumn };
            uiElement.SetBinding(TimeSpanEdit.TextDecorationsProperty, bind);            
#endif
        }
        #endregion
    }
}
