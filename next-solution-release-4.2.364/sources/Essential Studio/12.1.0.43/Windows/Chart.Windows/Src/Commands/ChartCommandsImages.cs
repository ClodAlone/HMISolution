#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Text;
using System.Drawing;
using System.Resources;
using System.Reflection;

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Contains commands icon (ToolBar, ContextMenu).
	/// </summary>
	sealed class ChartCommandsImages
	{
		#region Constants
		private const string c_resourceName = "Syncfusion.Windows.Forms.Chart.Commands.ChartCommandsImages";
		#endregion

		#region Members
		private readonly static Image c_saveImage;
		private readonly static Image c_copyImage;
		private readonly static Image c_printImage;
		private readonly static Image c_printPreviewImage;
		private readonly static Image c_paletteImage;
		private readonly static Image c_styleImage;
		private readonly static Image c_seriesTypeImage;
		private readonly static Image c_series3DImage;
		private readonly static Image c_series2DImage;
		private readonly static Image c_showLegendImage;
		private readonly static Image c_xZoomingImage;
		private readonly static Image c_yZoomingImage;
		private readonly static Image c_resetZoomingImage;
		private readonly static Image c_zoomingImage;
		private readonly static Image c_autoHighlight;
		private readonly static Image c_allowAlignment;
		private readonly static Image c_zoomOut;
		private readonly static Image c_zoomIn;
		private readonly static Image c_panning;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the save image.
		/// </summary>
		/// <value>The save.</value>
		public static Image Save
		{
			get
			{
				return c_saveImage;
			}
		}
		/// <summary>
		/// Gets the copy image.
		/// </summary>
		/// <value>The copy.</value>
		public static Image Copy
		{
			get
			{
				return c_copyImage;
			}
		}
		/// <summary>
		/// Gets the print image.
		/// </summary>
		/// <value>The print.</value>
		public static Image Print
		{
			get
			{
				return c_printImage;
			}
		}
		/// <summary>
		/// Gets the print image.
		/// </summary>
		/// <value>The print.</value>
		public static Image PrintPreview
		{
			get
			{
				return c_printPreviewImage;
			}
		}
		/// <summary>
		/// Gets the palettes image.
		/// </summary>
		/// <value>The palettes.</value>
		public static Image Palettes
		{
			get
			{
				return c_paletteImage;
			}
		}
		/// <summary>
		/// Gets the style image.
		/// </summary>
		/// <value>The style.</value>
		public static Image Style
		{
			get
			{
				return c_styleImage;
			}
		}
		/// <summary>
		/// Gets the type of the series image.
		/// </summary>
		/// <value>The type of the series.</value>
		public static Image SeriesType
		{
			get
			{
				return c_seriesTypeImage;
			}
		}
		/// <summary>
		/// Gets the series3D image.
		/// </summary>
		/// <value>The series3D.</value>
		public static Image Series3D
		{
			get
			{
				return c_series3DImage;
			}
		}
		/// <summary>
		/// Gets the series3D image.
		/// </summary>
		/// <value>The series3D.</value>
		public static Image Series2D
		{
			get
			{
				return c_series2DImage;
			}
		}
		/// <summary>
		/// Gets the show legend image.
		/// </summary>
		/// <value>The show legend.</value>
		public static Image ShowLegend
		{
			get
			{
				return c_showLegendImage;
			}
		}
		/// <summary>
		/// Gets the series3D image.
		/// </summary>
		/// <value>The series3D.</value>
		public static Image XZooming
		{
			get
			{
				return c_xZoomingImage;
			}
		}
		/// <summary>
		/// Gets the show legend image.
		/// </summary>
		/// <value>The show legend.</value>
		public static Image YZooming
		{
			get
			{
				return c_yZoomingImage;
			}
		}
		/// <summary>
		/// Gets the show legend image.
		/// </summary>
		/// <value>The show legend.</value>
		public static Image ResetZooming
		{
			get
			{
				return c_resetZoomingImage;
			}
		}
		/// <summary>
		/// Gets the show legend image.
		/// </summary>
		/// <value>The show legend.</value>
		public static Image Zooming
		{
			get
			{
				return c_zoomingImage;
			}
		}
		/// <summary>
		/// Gets the auto highlight.
		/// </summary>
		/// <value>The auto highlight.</value>
		public static Image AutoHighlight
		{
			get
			{
				return c_autoHighlight;
			}
		}
		/// <summary>
		/// Gets the allow alignment.
		/// </summary>
		/// <value>The allow alignment.</value>
		public static Image AllowAlignment
		{
			get
			{
				return c_allowAlignment;
			}
		}
		/// <summary>
		/// Gets the allow alignment.
		/// </summary>
		/// <value>The allow alignment.</value>
		public static Image ZoomOut
		{
			get
			{
				return c_zoomOut;
			}
		}
		/// <summary>
		/// Gets the allow alignment.
		/// </summary>
		/// <value>The allow alignment.</value>
		public static Image ZoomIn
		{
			get
			{
				return c_zoomIn;
			}
		}
		/// <summary>
		/// Gets the panning icon
		/// </summary>
		/// <value>The panning icon.</value>
		public static Image Panning
		{
			get
			{
				return c_panning;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
        /// Initializes the <see cref="ChartCommandsImages"/> class.
		/// </summary>
		static ChartCommandsImages()
		{
			ResourceManager manager = new ResourceManager(c_resourceName, Assembly.GetAssembly(typeof(ChartCommandsImages)));

			c_saveImage = manager.GetObject("Save.png") as Image;
			c_copyImage = manager.GetObject("Copy.png") as Image;
			c_printImage = manager.GetObject("Print.png") as Image;
			c_printPreviewImage = manager.GetObject("PrintPreview.png") as Image;
			c_paletteImage = manager.GetObject("Palette.png") as Image;
			c_styleImage = manager.GetObject("Properties.png") as Image;
			c_seriesTypeImage = manager.GetObject("Type.png") as Image;
			c_series3DImage = manager.GetObject("3dMode.png") as Image;
			c_series2DImage = manager.GetObject("2dMode.png") as Image;
			c_showLegendImage = manager.GetObject("Legend.png") as Image;
			c_xZoomingImage = manager.GetObject("XZooming.png") as Image;
			c_yZoomingImage = manager.GetObject("YZooming.png") as Image;
			c_resetZoomingImage = manager.GetObject("ResetZoom.png") as Image;
			c_zoomingImage = manager.GetObject("Zooming.png") as Image;
			c_autoHighlight = manager.GetObject("HighLight.png") as Image;
			c_allowAlignment = manager.GetObject("AllowAlignment.png") as Image;
			c_zoomOut = manager.GetObject("ZoomOut.png") as Image;
			c_zoomIn = manager.GetObject("ZoomIn.png") as Image;
			c_panning = manager.GetObject("Panning.png") as Image;
		}
		#endregion
	}
}
