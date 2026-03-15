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
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;
#if !WinRT
using Syncfusion.Windows.Styles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Syncfusion.Windows.Controls.Cells;
namespace Syncfusion.Windows.Controls.Grid
{
#else
using Syncfusion.WinRT.Styles;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Syncfusion.WinRT.Controls.Cells;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;
namespace Syncfusion.WinRT.Controls.Grid
{
#endif
    /// <summary>
    /// Defines possible error types.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Error information.
        /// </summary>
        Information = 1,
        /// <summary>
        /// Error message.
        /// </summary>
        ErrorMessage = 2,
        /// <summary>
        /// Custom.
        /// </summary>
        Custom = 3
    }

    /// <summary>
    /// Used to display error tool tips for the cells. 
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridErrorStyleInfo : StyleInfoSubObjectBase
    {
        // Static Fields
        private static GridErrorStyleInfo defaultErrorStyleInfo = null;

        /// <summary>
        /// Initalizes a new <see cref="GridErrorStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridErrorStyleInfo"/>.
        /// </param>
        [DebuggerStepThrough()]
        public GridErrorStyleInfo(StyleInfoSubObjectIdentity identity)
            : base(identity, new GridErrorStyleInfoStore())
        {
        }

        /// <summary>
        /// Initalizes a new <see cref="GridErrorStyleInfo"/> object and associates it with an existing <see cref="GridStyleInfoSubObjectIdentity"/>.
        /// </summary>
        /// <param name="identity">A <see cref="GridStyleInfoSubObjectIdentity"/> that holds the indentity for this <see cref="GridErrorStyleInfo"/>.</param>
        /// <param name="store">A <see cref="GridErrorStyleInfoStore"/> that holds data for this <see cref="GridErrorStyleInfo"/>.
        /// All changes in this style object will be saved in the <see cref="GridErrorStyleInfoStore"/> object.</param>
        [DebuggerStepThrough()]
        public GridErrorStyleInfo(StyleInfoSubObjectIdentity identity, GridErrorStyleInfoStore store)
            : base(identity, store)
        {
        }

        /// <summary>
        /// Initializes a new empty <see cref="GridErrorStyleInfo"/> object.
        /// </summary>
        [DebuggerStepThrough()]
        public GridErrorStyleInfo()
            : base(new GridErrorStyleInfoStore())
        {
        }

        internal static object CreateObject(StyleInfoSubObjectIdentity identity, object store)
        {
            if (store != null)
            {
                return new GridErrorStyleInfo(identity, store as GridErrorStyleInfoStore);
            }

            return new GridErrorStyleInfo(identity);
        }

        /// <summary>
        /// Gets the default style for <see cref="GridErrorStyleInfo"/> object.
        /// </summary>
        public static GridErrorStyleInfo Default
        {
            get
            {
                if (defaultErrorStyleInfo == null)
                {
                    defaultErrorStyleInfo = new GridErrorStyleInfo();
                    defaultErrorStyleInfo.ErrorType = ErrorType.Information;
                    defaultErrorStyleInfo.ErrorContentAlignment = ImageContentAlignment.Right;
#if !WinRT
                    defaultErrorStyleInfo.ErrorTooltipBackgroundBrush = Brushes.Red;
                    defaultErrorStyleInfo.ErrorTooltipForegroundBrush = Brushes.White;
#else
                    defaultErrorStyleInfo.ErrorTooltipBackgroundBrush = new SolidColorBrush(Colors.Red);
                    defaultErrorStyleInfo.ErrorTooltipForegroundBrush = new SolidColorBrush(Colors.White);
#endif
                    defaultErrorStyleInfo.ImageHeight = new GridLength(1.0d, GridUnitType.Star);
                    defaultErrorStyleInfo.ImageWidth = new GridLength(0.2d, GridUnitType.Star);
#if WPF
                    defaultErrorStyleInfo.ImageMargins = new CellMarginsInfo(0);
#else
                    defaultErrorStyleInfo.ImageMargins = new CellMarginsInfo(0, 0, 0, 0);
#endif
                }

                return defaultErrorStyleInfo;
            }
        }

        /// <summary>
        /// Returns <see cref="GridErrorStyleInfo.Default"/>.
        /// </summary>
        /// <returns>A <see cref="GridErrorStyleInfo"/> object with default values.</returns>
        protected override StyleInfoBase GetDefaultStyle()
        {
            return Default;
        }

        /// <summary>
        /// Creates a product-specific identity object for a sub object.
        /// </summary>
        /// <param name="sip">Sub object.</param>
        /// <returns>An identity object for a subobject of this style.</returns>
        public override StyleInfoSubObjectIdentity CreateSubObjectIdentity(StyleInfoProperty sip)
        {
            return new CachedStyleInfoSubObjectIdentity(this, sip);
        }

        /// <summary>
        /// Adjusts the margin of error info display according to the client area.
        /// </summary>
        /// <param name="defaultMargin">Default margin.</param>
        /// <param name="clientSize">Client size.</param>
        /// <returns>Adjusted margin.</returns>
        public Thickness AdjustErrorInfoMargin(Thickness defaultMargin, Size clientSize)
        {
            if (this.HasErrorMessage)
            {
                var width = (this.GetImageWidth() * clientSize.Width) + 2d;
                if (this.ReadOnlyErrorContentAlignment == ImageContentAlignment.Left)
                {
                    return new Thickness(defaultMargin.Left + width, defaultMargin.Top, defaultMargin.Right, defaultMargin.Bottom);
                }
                else
                {
                    return new Thickness(defaultMargin.Left, defaultMargin.Top, defaultMargin.Right + width, defaultMargin.Bottom);
                }
            }

            return defaultMargin;
        }

        internal Thickness AdjustErrorInfoMarginOnEditing(Thickness defaultMargin, Size clientSize)
        {
            //if (this.HasErrorMessage)
            {
                var width = (this.GetImageWidth() * clientSize.Width) + 2d;
                if (this.ReadOnlyErrorContentAlignment == ImageContentAlignment.Left)
                {
                    return new Thickness(defaultMargin.Left + width, defaultMargin.Top, defaultMargin.Right, defaultMargin.Bottom);
                }
                else
                {
                    return new Thickness(defaultMargin.Left, defaultMargin.Top, defaultMargin.Right + width, defaultMargin.Bottom);
                }
            }

            // return defaultMargin; Unreachable code
        }

        /// <summary>
        /// Adjusts the margin of error info display according to the client area.
        /// </summary>
        /// <param name="defaultMargin">Default margin.</param>
        /// <param name="grid">The grid.</param>
        /// <param name="rowColIndex">Row index.</param>
        /// <returns>Adjusted margin.</returns>
        public Thickness AdjustErrorInfoMargin(Thickness defaultMargin, GridControlBase grid, RowColumnIndex rowColIndex)
        {
#if !WinRT
            var rect = grid.RangeToRect(Syncfusion.Windows.Controls.Scroll.ScrollAxisRegion.Body, Syncfusion.Windows.Controls.Scroll.ScrollAxisRegion.Body, GridRangeInfo.Cell(rowColIndex.RowIndex, rowColIndex.ColumnIndex), false, false);
#else
            var rect = grid.RangeToRect(Syncfusion.WinRT.Controls.Scroll.ScrollAxisRegion.Body, Syncfusion.WinRT.Controls.Scroll.ScrollAxisRegion.Body, GridRangeInfo.Cell(rowColIndex.RowIndex, rowColIndex.ColumnIndex), false, false);
#endif
#if WPF
            return this.AdjustErrorInfoMargin(defaultMargin, rect.Size);
#else
            return this.AdjustErrorInfoMargin(defaultMargin, new Size(rect.Width, rect.Height));
#endif
        }

        public Thickness AdjustErrorInfoMarginOnEditing(Thickness defaultMargin, GridControlBase grid, RowColumnIndex rowColIndex)
        {
            if (grid.CurrentCell.IsEditing && !grid.Model.Options.ShowErrorIconOnEditing)
                return defaultMargin;
#if !WinRT
            var rect = grid.RangeToRect(Syncfusion.Windows.Controls.Scroll.ScrollAxisRegion.Body, Syncfusion.Windows.Controls.Scroll.ScrollAxisRegion.Body, GridRangeInfo.Cell(rowColIndex.RowIndex, rowColIndex.ColumnIndex), false, false);
#else
            var rect = grid.RangeToRect(Syncfusion.WinRT.Controls.Scroll.ScrollAxisRegion.Body, Syncfusion.WinRT.Controls.Scroll.ScrollAxisRegion.Body, GridRangeInfo.Cell(rowColIndex.RowIndex, rowColIndex.ColumnIndex), false, false);
#endif
#if WPF
            return this.AdjustErrorInfoMarginOnEditing(defaultMargin, rect.Size);
#else
            return this.AdjustErrorInfoMargin(defaultMargin, new Size(rect.Width, rect.Height));
#endif
        }

        #region ErrorType

        /// <summary>
        /// Returns the read only error type.
        /// </summary>
        public ErrorType ReadOnlyErrorType
        {
            get
            {
                if (this.HasErrorType)
                {
                    return this.ErrorType;
                }

                return Default.ErrorType;
            }
        }

        /// <summary>
        /// Gets or sets the Error Type.
        /// </summary>
        public ErrorType ErrorType
        {
            get
            {
                return (ErrorType)this.GetValue(GridErrorStyleInfoStore.ErrorTypeProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorTypeProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorType property.
        /// </summary>
        public void ResetErrorType()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorTypeProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorType property is serializable.
        /// </summary>
        /// <returns>True if it can be serialized.</returns>
        private bool ShouldSerializeErrorType()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorTypeProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorType property is initialized.
        /// </summary>
        public bool HasErrorType
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorTypeProperty);
            }
        }

        #endregion

        #region ErrorContentAlignment

        /// <summary>
        /// Returns the error content alignment.
        /// </summary>
        public ImageContentAlignment ReadOnlyErrorContentAlignment
        {
            get
            {
                if (this.HasErrorContentAlignment)
                {
                    return this.ErrorContentAlignment;
                }

                return Default.ErrorContentAlignment;
            }
        }

        /// <summary>
        /// Gets or sets the error content alignment.
        /// </summary>
        public ImageContentAlignment ErrorContentAlignment
        {
            get
            {
                return (ImageContentAlignment)this.GetValue(GridErrorStyleInfoStore.ErrorContentAlignmentProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorContentAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorContentAlignment property.
        /// </summary>
        public void ResetErrorContentAlignment()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorContentAlignmentProperty);
        }


        private bool ShouldSerializeErrorContentAlignment()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorContentAlignmentProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorContentAlignment property is initialized.
        /// </summary>
        public bool HasErrorContentAlignment
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorContentAlignmentProperty);
            }
        }

        #endregion

        #region CustomImage
        /// <summary>
        /// Gets or sets a custom image to use in the place of error icon.
        /// </summary>
        public BitmapImage CustomImage
        {
            get
            {
                return (BitmapImage)this.GetValue(GridErrorStyleInfoStore.ErrorCustomImageProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorCustomImageProperty, value);
            }
        }

        /// <summary>
        /// Resets the CustomImage property.
        /// </summary>
        public void ResetCustomBrush()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorCustomImageProperty);
        }

        private bool ShouldSerializeCustomBrush()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorCustomImageProperty);
        }

        /// <summary>
        /// Specifies whether the CustomImage property is initialized.
        /// </summary>
        public bool HasCustomImage
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorCustomImageProperty);
            }
        }

        #endregion

        #region ErrorMessage
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return (string)this.GetValue(GridErrorStyleInfoStore.ErrorMessageProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorMessageProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorMessage property.
        /// </summary>
        public void ResetErrorMessage()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorMessageProperty);
        }

        private bool ShouldSerializeErrorMessage()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorMessageProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorMessage property is initialized.
        /// </summary>
        public bool HasErrorMessage
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorMessageProperty);
            }
        }

        #endregion

        #region ErrorTooltipTemplateKey
        /// <summary>
        /// Gets or sets the template for the error tooltip.
        /// </summary>
        public string ErrorTooltipTemplateKey
        {
            get
            {
                return (string)this.GetValue(GridErrorStyleInfoStore.ErrorTooltipTemplateKeyProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorTooltipTemplateKeyProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorTooltipTemplateKey property.
        /// </summary>
        public void ResetErrorTooltipTemplateKey()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorTooltipTemplateKeyProperty);
        }

        private bool ShouldSerializeErrorTooltipTemplateKey()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipTemplateKeyProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorTooltipTemplateKey property is initialized.
        /// </summary>
        public bool HasErrorTooltipTemplateKey
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipTemplateKeyProperty);
            }
        }

        #endregion

        #region ErrorTooltipBackgroundBrush
        /// <summary>
        /// Gets or sets the background for error tooltip.
        /// </summary>
        public Brush ErrorTooltipBackgroundBrush
        {
            get
            {
                return (Brush)this.GetValue(GridErrorStyleInfoStore.ErrorTooltipBackgroundBrushProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorTooltipBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorTooltipBackgroundBrush property.
        /// </summary>
        public void ResetErrorTooltipBackgroundBrush()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorTooltipBackgroundBrushProperty);
        }

        private bool ShouldSerializeErrorTooltipBackgroundBrush()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipBackgroundBrushProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorTooltipBackgroundBrush property is initialized.
        /// </summary>
        public bool HasErrorTooltipBackgroundBrush
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipBackgroundBrushProperty);
            }
        }

        public Brush ReadOnlyErrorTooltipBackgroundBrush
        {
            get
            {
                if (this.HasErrorTooltipBackgroundBrush)
                {
                    return this.ErrorTooltipBackgroundBrush;
                }

                return Default.ErrorTooltipBackgroundBrush;
            }
        }

        #endregion

        #region ErrorTooltipForegroundBrush
        /// <summary>
        /// Gets or sets the foreground for error tooltip.
        /// </summary>
        public Brush ErrorTooltipForegroundBrush
        {
            get
            {
                return (Brush)this.GetValue(GridErrorStyleInfoStore.ErrorTooltipForegroundBrushProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ErrorTooltipForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Resets the ErrorTooltipForegroundBrush property.
        /// </summary>
        public void ResetErrorTooltipForegroundBrush()
        {
            this.ResetValue(GridErrorStyleInfoStore.ErrorTooltipForegroundBrushProperty);
        }

        private bool ShouldSerializeErrorTooltipForegroundBrush()
        {
            return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipForegroundBrushProperty);
        }

        /// <summary>
        /// Specifies whether the ErrorTooltipForegroundBrush property is initialized.
        /// </summary>
        public bool HasErrorTooltipForegroundBrush
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ErrorTooltipForegroundBrushProperty);
            }
        }

        public Brush ReadOnlyErrorTooltipForegroundBrush
        {
            get
            {
                if (this.HasErrorTooltipForegroundBrush)
                {
                    return this.ErrorTooltipForegroundBrush;
                }

                return Default.ErrorTooltipForegroundBrush;
            }
        }

        #endregion

        #region ImageWidth
        /// <summary>
        /// Gets or sets the width of the error icon.
        /// </summary>
        public GridLength ImageWidth
        {
            get
            {
                return (GridLength)this.GetValue(GridErrorStyleInfoStore.ImageWidthProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ImageWidthProperty, value);
            }
        }

        /// <summary>
        /// Resets the ImageWidth property.
        /// </summary>
        public void ResetImageWidth()
        {
            this.ResetValue(GridErrorStyleInfoStore.ImageWidthProperty);
        }

        /// <summary>
        /// Specifies whether the ImageWidth property is initialized.
        /// </summary>
        public bool HasImageWidth
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ImageWidthProperty);
            }
        }

        public GridLength ReadOnlyImageWidth
        {
            get
            {
                if (this.HasImageWidth)
                {
                    return this.ImageWidth;
                }

                return Default.ImageWidth;
            }
        }

        public double GetImageWidth()
        {
            var value = 0d;
            if (this.ReadOnlyImageWidth.IsStar)
            {
                value = this.ReadOnlyImageWidth.Value;
            }
            return value;
        }

        #endregion

        #region ImageHeight
        /// <summary>
        /// Gets or sets the height of the error icon.
        /// </summary>
        public GridLength ImageHeight
        {
            get
            {
                return (GridLength)this.GetValue(GridErrorStyleInfoStore.ImageHeightProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ImageHeightProperty, value);
            }
        }

        /// <summary>
        /// Resets the ImageHeight property.
        /// </summary>
        public void ResetImageHeight()
        {
            this.ResetValue(GridErrorStyleInfoStore.ImageHeightProperty);
        }

        /// <summary>
        /// Specifies whether the ImageHeight property is initialized.
        /// </summary>
        public bool HasImageHeight
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ImageHeightProperty);
            }
        }

        public GridLength ReadOnlyImageHeight
        {
            get
            {
                if (this.HasImageHeight)
                {
                    return this.ImageHeight;
                }

                return Default.ImageHeight;
            }
        }

        public double GetImageHeight()
        {
            var value = 0d;
            if (this.ReadOnlyImageHeight.IsStar)
            {
                value = this.ReadOnlyImageHeight.Value;
            }
            return value;
        }

        #endregion

        /// <summary>
        /// Defines the margins for the error icon.
        /// </summary>
        public CellMarginsInfo ImageMargins
        {
            get
            {
                return (CellMarginsInfo)this.GetValue(GridErrorStyleInfoStore.ImageMarginsProperty);
            }

            set
            {
                this.SetValue(GridErrorStyleInfoStore.ImageMarginsProperty, value);
            }
        }

        /// <summary>
        /// Resets the ImageMargins property.
        /// </summary>
        public void ResetImageMargins()
        {
            this.ResetValue(GridErrorStyleInfoStore.ImageMarginsProperty);
        }

        /// <summary>
        /// Specifies whether the ImageMargins property is initialized.
        /// </summary>
        public bool HasImageMargins
        {
            get
            {
                return this.HasValue(GridErrorStyleInfoStore.ImageMarginsProperty);
            }
        }

        /// <summary>
        /// Gets the margins of error image.
        /// </summary>
        public CellMarginsInfo ReadOnlyImageMargins
        {
            get
            {
                if (this.HasImageMargins)
                {
                    return this.ImageMargins;
                }

                return Default.ImageMargins;
            }
        }

        /// <summary>
        /// Adjusts the image margins according to the client rectangle.
        /// </summary>
        /// <param name="rectangle">Client rectangle.</param>
        /// <returns>Adjusted margins.</returns>
        public Rect AdjustImageMargins(Rect rectangle)
        {
            var right = this.ReadOnlyImageMargins.Right;
            var bottom = this.ReadOnlyImageMargins.Bottom;
            if (right < rectangle.Width && bottom < rectangle.Height)
            {
                return new Rect(rectangle.X + this.ReadOnlyImageMargins.Left, rectangle.Y + this.ReadOnlyImageMargins.Top, rectangle.Width - right, rectangle.Height - bottom);
            }
            return new Rect(rectangle.X + this.ReadOnlyImageMargins.Left, rectangle.Y + this.ReadOnlyImageMargins.Top, rectangle.Width, rectangle.Height);
        }
    }

    /// <summary>
    /// Implements the data store for the <see cref="GridMaskEditInfo"/> object.
    /// </summary>
    /// <seealso cref="StyleInfoStore"/>
    [
#if (!SILVERLIGHT && !WinRT)
Serializable,
#endif
 StaticDataField("sd")
]
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridErrorStyleInfoStore : StyleInfoStore
    {
        static StaticData sd = new StaticData(typeof(GridErrorStyleInfoStore), typeof(GridErrorStyleInfo), true);

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorType"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorTypeProperty = sd.CreateStyleInfoProperty(typeof(ErrorType), "ErrorType");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorContentAlignment"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorContentAlignmentProperty = sd.CreateStyleInfoProperty(typeof(ImageContentAlignment), "ErrorContentAlignment");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.CustomImage"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorCustomImageProperty = sd.CreateStyleInfoProperty(typeof(BitmapImage), "CustomImage");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorMessage"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorMessageProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorMessage");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorTooltipTemplateKey"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorTooltipTemplateKeyProperty = sd.CreateStyleInfoProperty(typeof(string), "ErrorTooltipTemplateKey");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorTooltipBackgroundBrush"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorTooltipBackgroundBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "ErrorTooltipBackgroundBrush");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ErrorTooltipForegroundBrush"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ErrorTooltipForegroundBrushProperty = sd.CreateStyleInfoProperty(typeof(Brush), "ErrorTooltipForegroundBrush");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ImageWidthProperty"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ImageWidthProperty = sd.CreateStyleInfoProperty(typeof(GridLength), "ImageWidth");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ImageHeightProperty"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ImageHeightProperty = sd.CreateStyleInfoProperty(typeof(GridLength), "ImageHeight");

        /// <summary>
        /// Provides information about the <see cref="GridErrorStyleInfo.ImageMargins"/> property. 
        /// </summary>
        public static readonly StyleInfoProperty ImageMarginsProperty = sd.CreateStyleInfoProperty(typeof(CellMarginsInfo), "ImageMargins");

        static GridErrorStyleInfoStore()
        {
            ErrorCustomImageProperty.SerializeXmlBehavior = SerializeXmlBehavior.Skip;
        }

        /// <overload>
        /// Initializes a new <see cref="GridErrorStyleInfoStore"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new empty <see cref="GridErrorStyleInfoStore"/>.
        /// </summary>
        public GridErrorStyleInfoStore()
        {
        }

#if !SyncfusionFramework4_0 && !WinRT
        /// <summary>
        /// Initializes a new <see cref="GridErrorStyleInfoStore"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        [System.Security.SecurityCritical()]
        protected GridErrorStyleInfoStore(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }
#endif
        /// <override/>
        protected override StaticData StaticDataStore
        {
            get { return sd; }
        }

        // Base class implementation of this method calls Activator.CreateInstance to achieve the same result.
        // Calling new directly is more efficient. Otherwise this override is obsolete.

        /// <override/>
        /// <summary>Returns a copy of current object.</summary>
        /// <returns>A copy of current object.</returns>
        public override object Clone()
        {
            StyleInfoStore target = new GridErrorStyleInfoStore();
            CopyTo(target);
            return target;
        }
    }
}
