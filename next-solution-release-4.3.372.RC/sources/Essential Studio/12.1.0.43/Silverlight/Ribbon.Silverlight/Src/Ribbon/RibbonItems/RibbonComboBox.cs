#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls 
{
	/// <summary>
	/// Represents a ribbon selection control with a drop-down list that can be shown or hidden by
	/// clicking the thumb on the control.
	/// </summary>
	/// <remarks>
	/// The <see cref="RibbonComboBox"/> allows the user to select an item from a drop-down list.
	/// </remarks>
	[TemplatePart(Name = "ContentPresenter", Type = typeof(ContentPresenter))]
	[TemplatePart(Name = "ContentPresenterBorder", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "DropDownToggle", Type = typeof(ToggleButton))]
	[TemplatePart(Name = "Popup", Type = typeof(Popup))]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "MouseOver")]
	[TemplateVisualState(GroupName = "CommonStates", Name = "Disabled")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Unfocused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "Focused")]
	[TemplateVisualState(GroupName = "FocusStates", Name = "FocusedDropDown")]
	public class RibbonComboBox :
        ComboBox, IRibbonControl
	{
		#region Fields

		private Popup popup;
		private Canvas outerCanvas;
		private FrameworkElement layoutRoot;
		private Storyboard aniMouseOver;
		private Storyboard aniHidePopup;
		private Storyboard aniOpenPopup;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RibbonComboBox"/> class.
		/// </summary>
		public RibbonComboBox()
		{
			this.DefaultStyleKey = typeof(RibbonComboBox);
		}

        /// <summary>
        /// Initializes the <see cref="RibbonComboBox"/> class.
        /// </summary>
        static RibbonComboBox()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                LoadDependentAssemblies load = new LoadDependentAssemblies();
                load = null;
            }
        }
		#endregion

		#region Overrides

		/// <summary>
		/// Builds the visual tree for the <see cref="T:System.Windows.Controls.ComboBox"/> when a new template is applied.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.layoutRoot = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
			this.aniMouseOver = this.layoutRoot.Resources["aniMouseOver"] as Storyboard;
			this.aniHidePopup = this.layoutRoot.Resources["aniHidePopup"] as Storyboard;
			this.aniOpenPopup = this.layoutRoot.Resources["aniOpenPopup"] as Storyboard;
			this.popup = (Popup)GetTemplateChild("Popup");

			if (this.popup != null)
			{
				Application.Current.Host.Content.Resized += new EventHandler(this.OnHostContentResized);
				this.popup.Opened += new EventHandler(this.OnPopupOpened);
				this.popup.Closed += new EventHandler(this.OnPopupClosed);

				Canvas popupCanvas = this.popup.Child as Canvas;

				if (popupCanvas != null)
				{
					UIElementCollection children = popupCanvas.Children;

					for (int i = children.Count - 1; i >= 0; --i)
					{
						Canvas outerCanvasOriginal = children[i] as Canvas;

						if (outerCanvasOriginal != null)
						{
							children.Remove(outerCanvasOriginal);

							this.outerCanvas = new ComboBoxOuterCanvas(this);

							children.Insert(i, this.outerCanvas);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Retrieves an empty instance of <see cref="RibbonComboBoxItem"/>.
		/// </summary>
		/// <returns>An empty combo box item.</returns>
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new RibbonComboBoxItem();
		}

		/// <summary>
		/// Determines whether the specified item is (or is eligible to be) its own item container.
		/// </summary>
		/// <param name="item">The item to evaluate.</param>
		/// <returns>
		/// <c>true</c> if the item is a <see cref="T:System.Windows.Controls.ComboBoxItem"/>; otherwise, <c>false</c>. The default is <c>false</c>.
		/// </returns>
		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return item is RibbonComboBoxItem;
		}

		/// <summary>
		/// Prepares the specified item element to display the specified item.
		/// </summary>
		/// <param name="element">Element used to display the specified item.</param>
		/// <param name="item">The item to display.</param>
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);

			RibbonComboBoxItem rcbItem = (RibbonComboBoxItem)element;

			rcbItem.Owner = this;
		}

		/// <summary>
		/// Provides handling for the <see cref="E:System.Windows.UIElement.MouseEnter"/> event that occurs when the mouse pointer enters this control.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

			this.StartMouseOverAnimation();
		}

		/// <summary>
		/// Provides handling for the <see cref="E:System.Windows.UIElement.MouseLeave"/> event that occurs when the mouse pointer leaves the combo box.
		/// </summary>
		/// <param name="e">The event data.</param>
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			this.StopMouseOverAnimation();
		}

        /// <summary>
        /// Arranges and sizes the combo box control and its contents.
        /// </summary>
        /// <param name="arrangeBounds">The size allowed for the combo box control.</param>
        /// <returns>The actual size of the combo box.</returns>
		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			Size size = base.ArrangeOverride(arrangeBounds);
			this.ArrangePopup();

			return size;
		}

        /// <summary>
        /// Arranges the popup.
        /// </summary>
		private void ArrangePopup()
		{
			FrameworkElement elementPopupChild = null;

			if (this.popup != null)
			{
				elementPopupChild = this.popup.Child as FrameworkElement;
			}

			FrameworkElement elementContentPresenterBorder = GetTemplateChild("ContentPresenterBorder") as FrameworkElement;
			ScrollViewer sv = GetTemplateChild("ScrollViewer") as ScrollViewer;

			if (((this.popup != null) && (elementPopupChild != null)) && ((elementContentPresenterBorder != null) && (this.outerCanvas != null)))
			{
				Content content = Application.Current.Host.Content;
				double contentActualWidth = content.ActualWidth;
				double contentActualHeight = content.ActualHeight;
				double popupActualWidth = elementPopupChild.ActualWidth;
				double popupActualHeight = elementPopupChild.ActualHeight;

				if (contentActualHeight != 0.0 && contentActualWidth != 0.0)
				{
					MatrixTransform transform = null;

					try
					{
						transform = elementContentPresenterBorder.TransformToVisual(null) as MatrixTransform;
					}
					catch
					{
						this.IsDropDownOpen = false;
					}

					if (transform != null)
					{
						double offsetX = transform.Matrix.OffsetX;
						double offsetY = transform.Matrix.OffsetY;
						double actualHeight = this.ActualHeight;
						double actualWidth = this.ActualWidth;
						double maxDropDownHeight = this.MaxDropDownHeight;

						if (double.IsInfinity(maxDropDownHeight) || double.IsNaN(maxDropDownHeight))
						{
							maxDropDownHeight = ((contentActualHeight - actualHeight) * 3.0) / 5.0;
                            maxDropDownHeight = maxDropDownHeight < 0 ? 0.0 : maxDropDownHeight;
						}

						popupActualWidth = Math.Min(popupActualWidth, contentActualWidth);
						popupActualHeight = Math.Min(popupActualHeight, maxDropDownHeight);
						popupActualWidth = Math.Max(actualWidth, popupActualWidth);

						double offsetX2 = offsetX;

						if (contentActualWidth < (offsetX2 + popupActualWidth))
						{
							offsetX2 = contentActualWidth - popupActualWidth;
							offsetX2 = Math.Max(0.0, offsetX2);
						}

						bool flag = true;
						double offsetY2 = offsetY + actualHeight;

						if (contentActualHeight < (offsetY2 + popupActualHeight))
						{
							flag = false;
							offsetY2 = offsetY - popupActualHeight;

							if (offsetY2 < 0.0)
							{
								if (offsetY < ((contentActualHeight - actualHeight) / 2.0))
								{
									flag = true;
									offsetY2 = offsetY + actualHeight;
								}
								else
								{
									flag = false;
									offsetY2 = offsetY - popupActualHeight;
								}
							}
						}

						if (flag)
						{
							maxDropDownHeight = Math.Min(contentActualHeight - offsetY2, maxDropDownHeight);
						}
						else
						{
							maxDropDownHeight = Math.Min(offsetY, maxDropDownHeight);
						}

						this.popup.HorizontalOffset = 0.0;
						this.popup.VerticalOffset = 0.0;
						this.outerCanvas.Width = contentActualWidth;
						this.outerCanvas.Height = contentActualHeight;

						Matrix matrix = transform.Matrix;

						this.InvertMatrix(ref matrix);

						transform.Matrix = matrix;
						this.outerCanvas.RenderTransform = transform;

						if (sv != null)
						{
							popupActualHeight = Math.Min(popupActualHeight, sv.ExtentHeight);
                            popupActualHeight = popupActualHeight < 0 ? 0.0 : popupActualHeight;
						}

						elementPopupChild.MinWidth = actualWidth;
						elementPopupChild.MaxWidth = contentActualWidth;
						elementPopupChild.MinHeight = Math.Min(actualHeight, popupActualHeight);
						elementPopupChild.MaxHeight = Math.Max(0.0, maxDropDownHeight);
						elementPopupChild.Width = popupActualWidth;
						elementPopupChild.Height = popupActualHeight;
						elementPopupChild.HorizontalAlignment = HorizontalAlignment.Left;
						elementPopupChild.VerticalAlignment = VerticalAlignment.Top;
                        
                        sv.MaxHeight = maxDropDownHeight;

						Canvas.SetLeft(elementPopupChild, offsetX2 - offsetX);
						Canvas.SetTop(elementPopupChild, offsetY2 - offsetY);
					}
				}
			}
		}

		#endregion

		#region Methods

		#endregion

		#region Implementation

        /// <summary>
        /// Starts the mouse over animation.
        /// </summary>
		private void StartMouseOverAnimation()
		{
			if (this.aniMouseOver != null && this.aniMouseOver.GetCurrentState() != ClockState.Active)
			{
				this.aniMouseOver.Begin();
			}
		}

        /// <summary>
        /// Stops the mouse over animation.
        /// </summary>
		private void StopMouseOverAnimation()
		{
			if (this.aniMouseOver != null)
			{
				this.aniMouseOver.Stop();
			}
		}

        /// <summary>
        /// Called when [popup opened].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnPopupOpened(object sender, EventArgs e)
		{
			this.ArrangeOuterCanvas();

			if (this.aniHidePopup != null && this.aniHidePopup.GetCurrentState() == ClockState.Active)
			{
				this.aniHidePopup.Begin();
			}

			if (this.aniOpenPopup != null && this.aniOpenPopup.GetCurrentState() != ClockState.Active)
			{
				this.aniOpenPopup.Begin();
			}
		}

        /// <summary>
        /// Called when [popup closed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnPopupClosed(object sender, EventArgs e)
		{
			if (this.aniOpenPopup != null && this.aniOpenPopup.GetCurrentState() == ClockState.Active)
			{
				this.aniOpenPopup.Stop();
			}
		}

        /// <summary>
        /// Arranges the outer canvas.
        /// </summary>
		private void ArrangeOuterCanvas()
		{
			if (this.outerCanvas == null || this.layoutRoot == null)
			{
				return;
			}

			this.popup.HorizontalOffset = 0.0;
			this.popup.VerticalOffset = 0.0;

			Content content = Application.Current.Host.Content;

			this.outerCanvas.Height = content.ActualHeight;
			this.outerCanvas.Width = content.ActualWidth;

			MatrixTransform transform = null;

			try
			{
				transform = (MatrixTransform)this.layoutRoot.TransformToVisual(null);
			}
			catch
			{
				this.IsDropDownOpen = false;
			}

			if (transform != null)
			{
				Matrix matrix = transform.Matrix;
				this.InvertMatrix(ref matrix);

				MatrixTransform t = new MatrixTransform();
				t.Matrix = matrix;

				this.outerCanvas.RenderTransform = t;
			}
		}

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <param name="m">The m.</param>
		private void InvertMatrix(ref Matrix m)
		{
			double num = (m.M11 * m.M22) - (m.M12 * m.M21);

			if (num != 0.0)
			{
				Matrix m2 = m;
				m.M11 = m2.M22 / num;
				m.M12 = (-1.0 * m2.M12) / num;
				m.M21 = (-1.0 * m2.M21) / num;
				m.M22 = m2.M11 / num;
				m.OffsetX = ((m2.OffsetY * m2.M21) - (m2.OffsetX * m2.M22)) / num;
				m.OffsetY = ((m2.OffsetX * m2.M12) - (m2.OffsetY * m2.M11)) / num;
			}
		}

        /// <summary>
        /// Closes the drop down with transition.
        /// </summary>
		internal void CloseDropDownWithTransition()
		{
			if (this.aniHidePopup != null)
			{
				this.aniHidePopup.Completed += new EventHandler(this.OnHidePopupAnimationCompleted);

				if (this.aniHidePopup.GetCurrentState() != ClockState.Active)
				{
					this.aniHidePopup.Begin();
				}
			}
		}

        /// <summary>
        /// Called when [hide popup animation completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnHidePopupAnimationCompleted(object sender, EventArgs e)
		{
			if (this.aniHidePopup != null)
			{
				this.aniHidePopup.Completed -= new EventHandler(this.OnHidePopupAnimationCompleted);
			}

			this.IsDropDownOpen = false;
		}

        /// <summary>
        /// Called when [host content resized].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void OnHostContentResized(object sender, EventArgs e)
		{
			if (this.popup != null && this.popup.IsOpen)
			{
				this.ArrangeOuterCanvas();
			}
		}

		#endregion

		#region Classes

        /// <summary>
        /// Represents the Combo Box Outer Canvas Class.
        /// </summary>
		private class ComboBoxOuterCanvas :
			Canvas
		{
			#region Fields

			private RibbonComboBox ribbonComboBox;

			#endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="ComboBoxOuterCanvas"/> class.
            /// </summary>
            /// <param name="ribbonComboBox">The ribbon combo box.</param>
			public ComboBoxOuterCanvas(RibbonComboBox ribbonComboBox)
			{
				this.ribbonComboBox = ribbonComboBox;

				this.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
				this.MouseLeftButtonDown += new MouseButtonEventHandler(this.OnMouseLeftButtonDown);
			}

            /// <summary>
            /// Called when [mouse left button down].
            /// </summary>
            /// <param name="sender">The sender.</param>
            /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
			private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
			{
				e.Handled = true;

				this.ribbonComboBox.CloseDropDownWithTransition();
			}
		}

		#endregion
	}
}