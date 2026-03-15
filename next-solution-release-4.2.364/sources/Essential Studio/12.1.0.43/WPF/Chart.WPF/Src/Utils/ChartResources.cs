// <copyright file="ChartResources.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Represents ChartResources class. Contains resources used in chart.
    /// </summary>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public static class ChartResources
    {
        #region Members
        /// <summary>
        /// Initializes m_actions
        /// </summary>
        private static readonly ImageSource m_actions = null;

        /// <summary>
        /// Initializes m_annotation
        /// </summary>
        private static readonly ImageSource m_annotation = null;

        /// <summary>
        /// Initializes m_series
        /// </summary>
        private static readonly ImageSource m_series = null;

        /// <summary>
        /// Initializes m_palettes
        /// </summary>
        private static readonly ImageSource m_palettes = null;

        /// <summary>
        /// Initializes toolBarItemSave
        /// </summary>
        private static readonly ImageSource toolBarItemSave = null;

        /// <summary>
        /// Initializes toolBarItemPrint
        /// </summary>
        private static readonly ImageSource toolBarItemPrint = null;

        /// <summary>
        /// Initializes toolBarItemSwitchPrint
        /// </summary>
        private static readonly ImageSource toolBarItemSwitchPrint = null;

        /// <summary>
        /// Initializes toolBarItemCopy
        /// </summary>
        private static readonly ImageSource toolBarItemCopy = null;

        /// <summary>
        /// Initializes toolBarItemColorPalette
        /// </summary>
        private static readonly ImageSource toolBarItemColorPalette = null;

        /// <summary>
        /// Initializes toolBarItemLegend
        /// </summary>
        private static readonly ImageSource toolBarItemLegend = null;

        /// <summary>
        /// Initializes toolBarItemType
        /// </summary>
        private static readonly ImageSource toolBarItemType = null;

        /// <summary>
        /// Initializes toolBarItemZoom
        /// </summary>
        private static readonly ImageSource toolBarItemZoom = null;

        /// <summary>
        /// Initializes toolBarItemProperties
        /// </summary>        
        private static readonly ImageSource toolBarItemProperties = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the actions image.
        /// </summary>
        /// <value>The actions.</value>
        public static ImageSource Actions
        {
            get
            {
                return m_actions;
            }
        }

        /// <summary>
        /// Gets the actions image.
        /// </summary>
        /// <value>The actions.</value>
        public static ImageSource annotation
        {
            get
            {
                return m_annotation;
            }
        }

        /// <summary>
        /// Gets the series image.
        /// </summary>
        /// <value>The series.</value>
        public static ImageSource Series
        {
            get
            {
                return m_series;
            }
        }

        /// <summary>
        /// Gets the palettes image.
        /// </summary>
        /// <value>The palettes.</value>
        public static ImageSource Palettes
        {
            get
            {
                return m_palettes;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Save image.
        /// </summary>
        /// <value>The ToolBarItemSave.</value>
        public static ImageSource ToolBarItemSave
        {
            get
            {
                return toolBarItemSave;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Zoom image.
        /// </summary>
        /// <value>The ToolBarItemZoom.</value>
        public static ImageSource ToolBarItemZoom
        {
            get
            {
                return toolBarItemZoom;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem SwitchPrint image.
        /// </summary>
        /// <value>The ToolBarItemSwitchPrint.</value>
        public static ImageSource ToolBarItemSwitchPrint
        {
            get
            {
                return toolBarItemSwitchPrint;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Print image.
        /// </summary>
        /// <value>The ToolBarItemPrint.</value>
        public static ImageSource ToolBarItemPrint
        {
            get
            {
                return toolBarItemPrint;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Copy image.
        /// </summary>
        /// <value>The ToolBarItemCopy.</value>
        public static ImageSource ToolBarItemCopy
        {
            get
            {
                return toolBarItemCopy;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem ColorPalette image.
        /// </summary>
        /// <value>The ToolBarItemColorPalette.</value>
        public static ImageSource ToolBarItemColorPalette
        {
            get
            {
                return toolBarItemColorPalette;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Legend image.
        /// </summary>
        /// <value>The ToolBarItemLegend.</value>
        public static ImageSource ToolBarItemLegend
        {
            get
            {
                return toolBarItemLegend;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Type image.
        /// </summary>
        /// <value>The ToolBarItemType.</value>
        public static ImageSource ToolBarItemType
        {
            get
            {
                return toolBarItemType;
            }
        }

        /// <summary>
        /// Gets the ToolBarItem Properties image.
        /// </summary>
        /// <value>The ToolBarItemProperties.</value>
        public static ImageSource ToolBarItemProperties
        {
            get
            {
                return toolBarItemProperties;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes static members of the <see cref="ChartResources"/> class.
        /// </summary>
        static ChartResources()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(ChartResources));

            m_actions = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Actions.bmp");
            m_series = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Series.bmp");
            m_palettes = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Palettes.bmp");
            m_annotation = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Annotation.png");
            toolBarItemSave = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Save.png");
            toolBarItemPrint = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.printer.png");
            toolBarItemCopy = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.copy.png");
            toolBarItemColorPalette = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.ColorPallete.png");
            toolBarItemLegend = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.ShowHide.png");
            toolBarItemType = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.ChartType.png");
            toolBarItemSwitchPrint = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Print Preview.png");
            toolBarItemZoom = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.view.png");
            toolBarItemProperties = LoadImage(assembly, "Syncfusion.Windows.Chart.Resources.Chart Edit setting.png");
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="imagename">The imagename.</param>
        /// <returns>The image to load</returns>
        private static BitmapImage LoadImage(Assembly assembly, string imagename)
        {
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.StreamSource = assembly.GetManifestResourceStream(imagename);
            image.EndInit();

            return image;
        }

        /// <summary>
        /// Loads the cursor.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="cursorname">The cursorname.</param>
        /// <returns>The cursor</returns>
        private static Cursor LoadCursor(Assembly assembly, string cursorname)
        {
            return new Cursor(assembly.GetManifestResourceStream(cursorname));
        }
        #endregion
    }
}
