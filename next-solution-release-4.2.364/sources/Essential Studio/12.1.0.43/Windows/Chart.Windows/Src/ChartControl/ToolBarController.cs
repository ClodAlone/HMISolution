#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// This class manages the toolbar of chart.
    /// </summary>
    /// <remarks>
    /// Use the above mentioned property in ChartControl to access the built-in controller and provide a custom toolbar 
    /// with custom buttons through the <see cref="SetToolBar"/> method.
    /// </remarks>

    [Obsolete("This class is obsolete.")]
    class ChartToolBarController : IDisposable
    {
        #region Constants
        private const string c_menuItemEditPaletteText = "Edit palette...";
        private const string c_menuItemSeparatorText = "-";
        private const string FILE_FILTERS = "Image file(*.bmp,*.jpeg,*.jpg,*.tiff,*.gif,*.emf)|*.bmp;*.jpeg;*.jpg;*.tiff;*.gif;*.emf|All files (*.*)|*.*";
        private const string VECTOR_FORMAT_FILTERS = "Metafile (*.emf)|*.emf|SVG files (*.svg)|*.svg|PostScript files (*.eps)|*.eps";
        #endregion

        #region Members
        private ContextMenu m_seriesMenu = new ContextMenu();
        private ContextMenu m_paletteMenu = new ContextMenu();
        private ContextMenu m_typeMenu = new ContextMenu();
        private ChartControl chart;
        private ChartToolBar toolBar;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBarController"/> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartToolBarController(ChartControl chart)
        {
            this.chart = chart;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Sets the ToolBar. Replaces the current toolbar with the specified one. If the specified toolbar already has a parent control,
        /// then just copy over the buttons to the current toolbar.
        /// </summary>
        /// <param name="toolBar">The ToolBar.</param>
        public void SetToolBar(ChartToolBar toolBar)
        {
            if ((this.toolBar != null))
            {
                if (toolBar.Parent != chart) // if toolbar's parent is not ChartControl then simply copy buttons 
                {
                    this.toolBar.Buttons.Clear();// = toolBar;
                    foreach (Button b in toolBar.Buttons)
                    {
                        this.toolBar.Buttons.Add(b);
                    }
                }
                else
                {
                    this.toolBar.Dispose();
                    this.toolBar = toolBar;
                }
            }
            else
            {
                this.toolBar = toolBar;
                OnToolBarSet();////setting default buttons
            }
        }

        /// <summary>
        /// Saves the chart as image.
        /// </summary>
        public void SaveChart()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = FILE_FILTERS;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                chart.SaveImage(dialog.FileName);
            }
        }

        /// <summary>
        /// Copies the chart to the clipboard.
        /// </summary>
        public void CopyChart()
        {
            Image img = new Bitmap((chart as Control).Width, (chart as Control).Height);
            chart.Draw(img);
            Clipboard.SetDataObject(img);
        }

        /// <summary>
        /// Displays series menu.
        /// </summary>
        /// <param name="control">The control.</param>
        public void EditStyle(Control control)
        {
            m_seriesMenu.MenuItems.Clear();

            for (int i = 0, end = chart.Series.Count; i < end; i++)
            {
                m_seriesMenu.MenuItems.Add(chart.Series[i].Name, new EventHandler(OnMenuClick));
            }

            m_seriesMenu.Show(control, new Point(0, 0));
        }

        /// <summary>
        /// Prints the chart.
        /// </summary>
        public void PrintChart()
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = chart.PrintDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    chart.PrintDocument.Print();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Displays palette menu.
        /// </summary>
        /// <param name="control">The control.</param>
        public void SelectPalette(Control control)
        {
            m_paletteMenu.MenuItems.Clear();
            Array palettes = Enum.GetValues(typeof(ChartColorPalette));

            for (int i = 0, end = palettes.Length; i < end; i++)
            {
                m_paletteMenu.MenuItems.Add(new TagMenuItem(palettes.GetValue(i), new EventHandler(OnPaletteMenuClick)));

                if (chart.Palette.Equals(palettes.GetValue(i)))
                {
                    m_paletteMenu.MenuItems[i].Checked = true;
                }
            }

            m_paletteMenu.MenuItems.Add(new MenuItem(c_menuItemSeparatorText));
            m_paletteMenu.MenuItems.Add(new MenuItem(this.chart.Localization.EditPalette, new EventHandler(OnEditPaletteMenuClick)));

            m_paletteMenu.Show(control, new Point(0, 0));
        }

        /// <summary>
        /// Displays the series types menu.
        /// </summary>
        /// <param name="control">The control.</param>
        public void SelectSeriesType(Control control)
        {
            bool isCombinedType = false;
            Array palettes = Enum.GetValues(typeof(ChartSeriesType));
            ChartSeriesType combinedType = ChartSeriesType.Line;

            if (chart.Series.Count > 0)
            {
                isCombinedType = true;
                combinedType = chart.Series[0].Type;

                for (int i = 1; i < chart.Series.Count; i++)
                {
                    if (chart.Series[i].Type != combinedType)
                    {
                        isCombinedType = false;
                        break;
                    }
                }
            }

            m_typeMenu.MenuItems.Clear();

            for (int i = 0, end = palettes.Length; i < end; i++)
            {
                m_typeMenu.MenuItems.Add(new TagMenuItem(palettes.GetValue(i), new EventHandler(OnTypeMenuClick)));

                if (isCombinedType)
                {
                    if (combinedType.Equals(palettes.GetValue(i)))
                    {
                        m_typeMenu.MenuItems[i].Checked = true;
                    }
                }
            }

            m_typeMenu.Show(control, new Point(0, 0));
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Set default buttons.
        /// </summary>
        private void SetDefaultButtons()
        {
            SetSaveButton();
            SetCopyButton();
            SetEditStyleButton();
            SetPrintButton();
            SetSelectPaletteButton();
            SetSelectTypeButton();
        }

        /// <summary>
        /// Sets the save button.
        /// </summary>
        private void SetSaveButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.Save.bmp");
            bttn.Click += new EventHandler(ButtonSaveClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Sets the copy button.
        /// </summary>
        private void SetCopyButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.Copy.bmp");
            bttn.Click += new EventHandler(ButtonCopyClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Sets the editstyle button.
        /// </summary>
        private void SetEditStyleButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.EditStyle.bmp");
            bttn.Click += new EventHandler(ButtonEditStyleClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Set print button.
        /// </summary>
        private void SetPrintButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.Print.bmp");
            bttn.Click += new EventHandler(ButtonPrintClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Set select palette button.
        /// </summary>
        private void SetSelectPaletteButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.Palettes.bmp");
            bttn.Click += new EventHandler(ButtonSelectPaletteClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Set select type button.
        /// </summary>
        private void SetSelectTypeButton()
        {
            Button bttn = new Button();

            bttn.Image = GetBitmap(typeof(ChartToolBar), "ToolBar.Images.SeriesType.bmp");
            bttn.Click += new EventHandler(ButtonSelectSeriesTypeClick);

            toolBar.Buttons.Add(bttn);
        }

        /// <summary>
        /// Method is called to set the default buttons.
        /// </summary>
        protected virtual void OnToolBarSet()
        {
            SetDefaultButtons();
        }

        /// <summary>
        /// Returns the image from a specified resource.
        /// </summary>
        /// <param name="type">The class used to extract the resource. </param>
        /// <param name="resource">The name of the resource.</param>
        /// <returns>Instance of the <see cref="Bitmap"/></returns>
        protected Bitmap GetBitmap(Type type, string resource)
        {
            Bitmap bmp = new Bitmap(type, resource);
            bmp.MakeTransparent(Color.Fuchsia);
            return bmp;
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Method is called when the Save button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonSaveClick(object sender, EventArgs e)
        {
            SaveChart();
        }

        /// <summary>
        /// Method is called when the Copy button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonCopyClick(object sender, EventArgs e)
        {
            CopyChart();
        }

        /// <summary>
        /// Method is called when the Edit button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonEditStyleClick(object sender, EventArgs e)
        {
            EditStyle(sender as Control);
        }

        /// <summary>
        /// Method is called when the Print button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonPrintClick(object sender, EventArgs e)
        {
            PrintChart();
        }

        /// <summary>
        /// Method is called when the Palettes button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonSelectPaletteClick(object sender, EventArgs e)
        {
            SelectPalette(sender as Control);
        }

        /// <summary>
        /// Method is called when the Series types button is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ButtonSelectSeriesTypeClick(object sender, EventArgs e)
        {
            SelectSeriesType(sender as Control);
        }

        /// <summary>
        /// Method is called when the series menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnMenuClick(object sender, EventArgs e)
        {
            chart.DisplayUserEditStylesDialog((sender as MenuItem).Index);
        }

        /// <summary>
        /// Method is called when the palettes menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnPaletteMenuClick(object sender, EventArgs e)
        {
            chart.Palette = (ChartColorPalette)(sender as TagMenuItem).Tag;
        }

        /// <summary>
        /// Method is called when the Edit palette item is clicked.
        /// </summary>
        /// <param name="sender">A sender of the event.</param>
        /// <param name="e">Argument.</param>
        private void OnEditPaletteMenuClick(object sender, EventArgs e)
        {
            PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.chart)["CustomPalette"];

            if (propDescriptor != null)
            {
                UITypeEditor uiEditor = (UITypeEditor)propDescriptor.GetEditor(typeof(UITypeEditor));

                if (uiEditor != null)
                {
                    TypeDescriptorContext typeDescriptorContext = new TypeDescriptorContext(this.chart.CustomPalette, propDescriptor);
                    WindowsFormsEditorServiceContainer windowsFormsEditorServiceContainer = new WindowsFormsEditorServiceContainer(typeDescriptorContext.ServiceProvider);

                    this.chart.CustomPalette = (Color[])uiEditor.EditValue(typeDescriptorContext, windowsFormsEditorServiceContainer, this.chart.CustomPalette);
                }
            }
        }

        /// <summary>
        /// Method is called when the palettes menu is clicked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTypeMenuClick(object sender, EventArgs e)
        {
            for (int i = 0; i < chart.Series.Count; i++)
            {
                chart.Series[i].Type = (ChartSeriesType)(sender as TagMenuItem).Tag;
            }
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_paletteMenu.Dispose();
            m_seriesMenu.Dispose();
            m_typeMenu.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
