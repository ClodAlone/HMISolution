#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Syncfusion.PdfViewer.Base;
using System.Drawing.Printing;
using Syncfusion.Pdf.Parsing;

namespace Syncfusion.Windows.Forms.PdfViewer
{
    /// <summary>
    /// Toolbar for the PdfViewerControl.
    /// </summary>
    [ToolboxItem(false)]
    public partial class DocumentToolbar : UserControl
    {
        #region Members
        PdfDocumentView m_activeView;
        int[] zoomValues = new int[]{
           50, 75, 100, 125, 150, 200, 400};
        #endregion

        #region Constructor
        /// <summary>
        /// Creates an instance DocumentToolbar.
        /// </summary>
        public DocumentToolbar()
        {
            InitializeComponent();

            InitializeDefaults();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the PdfDocumentView associated with this instance of DocumentToolbar.
        /// </summary>
        public PdfDocumentView ActiveView
        {
            get
            {
                return m_activeView;
            }
            set
            {
                if (m_activeView == null)
                {
                    m_activeView = value;
                }
            }
        }
        #endregion

        #region Helper Methods
        internal void InitializeDefaults()
        {
            WireUpEvents();

            toolStripButton1.Click += new EventHandler(OpenFile);
            txtCurrentPageIndex.Text = "0";
            txtCurrentPageIndex.Enabled = false;
            lblTotalPageCount.Text = "0";
            lblTotalPageCount.Enabled = false;

            btnGoToFirstPage.Enabled = false;
            btnGoToPreviousPage.Enabled = false;
            btnGoToNextPage.Enabled = false;
            btnGoToLastPage.Enabled = false;
            btnZoomIn.Enabled = false;
            btnZoomOut.Enabled = false;
            cmbCurrentZoomLevel.Enabled = false;
            btnFitPage.Enabled = false;
            btnFitWidth.Enabled = false;


            btnPrint.Enabled = false;
            btnSave.Enabled = false;

        }
        /// <summary>
        /// Initializes DocumentToolbar for the document loaded in PdfDocumentView.
        /// </summary>
        public void Initialize(PdfDocumentView view)
        {
            view.AltPageCount = 0;
            m_activeView = view;

            m_activeView.NavigationButtonStatesChanged += new PdfDocumentView.NavigationButtonStatesChangedEventHandler(m_activeView_NavigationButtonStatesChanged);
            m_activeView.CurrentPageChanged += new PdfDocumentView.CurrentPageChangedEventHandler(m_activeView_CurrentPageChanged);

            btnPrint.Enabled = true;
            txtCurrentPageIndex.Enabled = true;
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
            cmbCurrentZoomLevel.Enabled = true;
            btnFitWidth.Enabled = true;
            btnFitPage.Enabled = true;
            btnSave.Enabled = true;

            SetCurrentPageIndex(m_activeView.CurrentPageIndex);
            lblTotalPageCount.Text = m_activeView.PageCount.ToString();

            m_activeView.ResetNavigationButtonStates();
            m_activeView_NavigationButtonStatesChanged(null, null);
            m_activeView.ZoomChanged += new PdfDocumentView.ZoomChangedEventHandler(m_activeView_ZoomChanged);
        }

        void m_activeView_ZoomChanged(object sender, int zoomFactor)
        {
            cmbCurrentZoomLevel.Text = string.Format("{0}%", zoomFactor);
        }
        void m_activeView_CurrentPageChanged(object sender, EventArgs args)
        {
            SetCurrentPageIndex(m_activeView.CurrentPageIndex);
        }

        void m_activeView_NavigationButtonStatesChanged(object sender, EventArgs args)
        {
            btnGoToFirstPage.Enabled = m_activeView.CanGoToFirstPage;
            btnGoToLastPage.Enabled = m_activeView.CanGoToLastPage;
            btnGoToNextPage.Enabled = m_activeView.CanGoToNextPage;
            btnGoToPreviousPage.Enabled = m_activeView.CanGoToPreviousPage;
        }

        void WireUpEvents()
        {
            btnGoToFirstPage.Click += new EventHandler(btnGoToFirstPage_Click);
            btnGoToPreviousPage.Click += new EventHandler(btnGoToPreviousPage_Click);
            btnGoToNextPage.Click += new EventHandler(btnGoToNextPage_Click);
            btnGoToLastPage.Click += new EventHandler(btnGoToLastPage_Click);
            btnPrint.Click += new EventHandler(btnPrint_Click);
            txtCurrentPageIndex.KeyDown += new KeyEventHandler(txtCurrentPageIndex_KeyDown);

            btnGoToFirstPage.EnabledChanged += new EventHandler(btnGoToFirstPage_EnabledChanged);
            btnGoToLastPage.EnabledChanged += new EventHandler(btnGoToLastPage_EnabledChanged);
            btnGoToNextPage.EnabledChanged += new EventHandler(btnGoToNextPage_EnabledChanged);
            btnGoToPreviousPage.EnabledChanged += new EventHandler(btnGoToPreviousPage_EnabledChanged);

            btnFitWidth.Click += new EventHandler(btnFitWidth_Click);
            btnFitPage.Click += new EventHandler(btnFitPage_Click);

            cmbCurrentZoomLevel.SelectedIndexChanged += new EventHandler(CurrentZoomLevelChanged);
            cmbCurrentZoomLevel.KeyDown += new KeyEventHandler(cmbCurrentZoomLevel_KeyDown);
            btnZoomIn.Click += new EventHandler(btnZoomIn_Click);
            btnZoomOut.Click += new EventHandler(btnZoomOut_Click);

            btnSave.Click += new EventHandler(btnSave_Click);
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (m_activeView != null)
            {
                if (m_activeView.LoadedDocument != null)
                {
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "PDF Files (*.pdf)|*.pdf";
                    save.AddExtension = true;
                    if (save.ShowDialog() == DialogResult.OK)
                    {
                        PdfLoadedDocument ldoc = m_activeView.LoadedDocument;
                        ldoc.Save(save.FileName);
                    }
                }
            }
        }

        void cmbCurrentZoomLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ToolStripControlHost toolStripControlHost = sender as ToolStripControlHost;
                string zoomEntered = toolStripControlHost.Text;
                int magnificationValue;
                if (zoomEntered.Contains("%"))
                {
                    int index = zoomEntered.IndexOf('%');
                    zoomEntered = zoomEntered.Substring(0, index);
                }
                int.TryParse(zoomEntered, out magnificationValue);
                if (magnificationValue < 50)
                    magnificationValue = 50;
                if (magnificationValue > 400)
                    magnificationValue = 400;

                m_activeView.ZoomTo(magnificationValue);
                cmbCurrentZoomLevel.Text = magnificationValue.ToString() + "%";
            }
        }

        void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Pdf files *.Pdf|*.Pdf";
            openFileDialog.Title = "Select a Pdf file.";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                m_activeView.Load(openFileDialog.FileName);
            }
            catch (Syncfusion.Pdf.PdfDocumentException)
            {
#if !SyncfusionFramework2_0
                PasswordToolBox passwordToolBox = new PasswordToolBox();
                passwordToolBox.ShowDialog();

                if (passwordToolBox.Password != string.Empty)
                {
                    try
                    {
                        m_activeView.Load(openFileDialog.FileName, passwordToolBox.Password);
                    }
                    catch (Syncfusion.Pdf.PdfDocumentException)
                    {
                        MessageBox.Show("Password is Invalid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
#endif
            }
            m_activeView.ZoomTo(100);
            cmbCurrentZoomLevel.Text = "100%";
            Initialize(m_activeView);
            this.lblTotalPageCount.Text = m_activeView.Pages.Length.ToString();
            NotificationBar notify = new NotificationBar(string.Empty);
        }

        void btnZoomOut_Click(object sender, EventArgs e)
        {
            int currentZoomLevel = GetCurrentZoomLevel();
            int index = cmbCurrentZoomLevel.Items.IndexOf(string.Format("{0}%", currentZoomLevel));
            if (index - 1 >= 0)
            {
                cmbCurrentZoomLevel.SelectedIndex = index - 1;
            }
            else
            {
                for (int i = 0; i < zoomValues.Length; i++)
                {
                    if (i + 1 < zoomValues.Length)
                    {
                        if (currentZoomLevel >= zoomValues[i] && currentZoomLevel < zoomValues[i + 1])
                        {
                            if (i == cmbCurrentZoomLevel.SelectedIndex)
                            {
                                CurrentZoomLevelChanged(this.cmbCurrentZoomLevel, EventArgs.Empty);
                            }
                            else
                                cmbCurrentZoomLevel.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }
        }

        void btnZoomIn_Click(object sender, EventArgs e)
        {
            int currentZoomLevel = GetCurrentZoomLevel();
            int index;
            if (currentZoomLevel < 50)
                index = 0;
            else
                index = cmbCurrentZoomLevel.Items.IndexOf(string.Format("{0}%", currentZoomLevel));
            if (index >= 0 && index + 1 < cmbCurrentZoomLevel.Items.Count)
            {
                cmbCurrentZoomLevel.SelectedIndex = index + 1;
            }
            else
            {
                for (int i = 0; i < zoomValues.Length; i++)
                {
                    if (i - 1 >= 0)
                    {
                        if (currentZoomLevel <= zoomValues[i] && currentZoomLevel > zoomValues[i - 1])
                        {
                            cmbCurrentZoomLevel.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }
        }

        int GetCurrentZoomLevel()
        {
            string text = "";
            text = cmbCurrentZoomLevel.Text;
            text = text.TrimEnd(new char[] { '%' });
            int zoomLevel = 100;

            if (int.TryParse(text, out zoomLevel))
            {
                return zoomLevel;
            }

            return 100;
        }

        void CurrentZoomLevelChanged(object sender, EventArgs e)
        {
            string text = cmbCurrentZoomLevel.SelectedItem.ToString();
            text = text.TrimEnd(new char[] { '%' });
            int zoomLevel = 100;

            if (int.TryParse(text, out zoomLevel))
            {
                m_activeView.ZoomMode = ZoomMode.Default;
                m_activeView.ZoomTo(zoomLevel);
            }
        }

        void btnFitPage_Click(object sender, EventArgs e)
        {
            m_activeView.ZoomMode = ZoomMode.FitPage;
        }

        void btnFitWidth_Click(object sender, EventArgs e)
        {
            m_activeView.ZoomMode = ZoomMode.FitWidth;
        }

        void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog dialog = new PrintDialog();

            int pageCount = m_activeView.Pages.Length;
            dialog.AllowPrintToFile = true;
            dialog.AllowSomePages = true;
            dialog.PrinterSettings.FromPage = 1;
            dialog.PrinterSettings.ToPage = pageCount;
            dialog.PrinterSettings.MaximumPage = pageCount;
            dialog.PrinterSettings.MinimumPage = 1;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                m_activeView.m_printFromPage = dialog.PrinterSettings.FromPage;
                m_activeView.m_printToPage = dialog.PrinterSettings.ToPage;
                System.Drawing.Printing.PrinterSettings.PaperSizeCollection collection = dialog.PrinterSettings.PaperSizes;
                string sourceName = dialog.PrinterSettings.PaperSources[0].SourceName;
                int rawKind = dialog.PrinterSettings.PaperSources[0].RawKind;

                foreach (System.Drawing.Printing.PaperSize size in collection)
                {
                    if (size.RawKind == rawKind)
                    {
                        m_activeView.m_printHeight = size.Height;
                        m_activeView.m_printWidth = size.Width;
                    }
                }

                PrintDocument document = m_activeView.PrintDocument;
                document.PrinterSettings = dialog.PrinterSettings;
                for (int i = 0; i < dialog.PrinterSettings.Copies; i++)
                {
                    m_activeView.m_currentPageOnPrint = dialog.PrinterSettings.FromPage - 1;
                    document.Print();
                }
            }
        }

        void btnGoToPreviousPage_EnabledChanged(object sender, EventArgs e)
        {
            btnGoToPreviousPage.Image =
                (btnGoToPreviousPage.Enabled)
                    ? Syncfusion.Windows.PdfViewer.Properties.Resources.GoToPreviousPage_Enabled
                    : Syncfusion.Windows.PdfViewer.Properties.Resources.GoToPreviousPage_Disabled;
        }

        void btnGoToNextPage_EnabledChanged(object sender, EventArgs e)
        {
            btnGoToNextPage.Image =
                (btnGoToNextPage.Enabled)
                    ? Syncfusion.Windows.PdfViewer.Properties.Resources.GoToNextPage_Enabled
                    : Syncfusion.Windows.PdfViewer.Properties.Resources.GoToNextPage_Disabled;
        }

        void btnGoToLastPage_EnabledChanged(object sender, EventArgs e)
        {
            btnGoToLastPage.Image =
                (btnGoToLastPage.Enabled)
                    ? Syncfusion.Windows.PdfViewer.Properties.Resources.GoToLastPage_Enabled
                    : Syncfusion.Windows.PdfViewer.Properties.Resources.GoToLastPage_Disabled;
        }

        void btnGoToFirstPage_EnabledChanged(object sender, EventArgs e)
        {
            btnGoToFirstPage.Image =
                (btnGoToFirstPage.Enabled)
                    ? Syncfusion.Windows.PdfViewer.Properties.Resources.GoToFirstPage_Enabled
                    : Syncfusion.Windows.PdfViewer.Properties.Resources.GoToFirstPage_Disabled;
        }

        void SetCurrentPageIndex(int pageIndex)
        {
            txtCurrentPageIndex.Text = pageIndex.ToString();
        }

        int GetCurrentPageIndex()
        {
            if (string.IsNullOrEmpty(txtCurrentPageIndex.Text))
                return 1;

            else
            {
                int index = 1;
                if (int.TryParse(txtCurrentPageIndex.Text, out index))
                    return index;
            }

            return -1;
        }


        void txtCurrentPageIndex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int index = GetCurrentPageIndex();

                if (index > m_activeView.Pages.Length)
                {
                    MessageBox.Show(
                        string.Format("There is no page numbered '{0}' in this document.", index), "Essential Pdf Viewer",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtCurrentPageIndex.Text = m_activeView.CurrentPageIndex.ToString();
                }
                else
                {
                    m_activeView.GoToPageAtIndex(index);
                }
            }
        }


        void btnGoToLastPage_Click(object sender, EventArgs e)
        {
            m_activeView.GoToLastPage();
        }

        void btnGoToNextPage_Click(object sender, EventArgs e)
        {
            m_activeView.GoToNextPage();
        }

        void btnGoToPreviousPage_Click(object sender, EventArgs e)
        {
            m_activeView.GoToPreviousPage();
        }

        void btnGoToFirstPage_Click(object sender, EventArgs e)
        {
            m_activeView.GoToFirstPage();
        }

        private void txtCurrentPageIndex_Click(object sender, EventArgs e)
        {
            txtCurrentPageIndex.SelectAll();
        }
        #endregion
    }
}
