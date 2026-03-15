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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.IO;
using System.Printing;
using System.Windows.Markup;
using Syncfusion.PdfViewer.Base;
using Syncfusion.Pdf.Parsing;
using System.Reflection;

namespace Syncfusion.Windows.PdfViewer
{
    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class DocumentToolbar : UserControl
    {
        #region Members
        PdfDocumentView m_activeView;
        private PdfViewerExceptions exceptions = new PdfViewerExceptions();
        int[] zoomValues = new int[]{
           10, 25, 50, 75, 100, 125, 150, 200, 400, 800, 1600, 2400, 3200, 6400};
        string filename = string.Empty;
        #endregion
      
        public DocumentToolbar()
        {
            InitializeComponent();
            AddHandler(PreviewMouseDownEvent,new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,new RoutedEventHandler(SelectAllText), true);
            btnOpen.Click+=new RoutedEventHandler(btnOpen_Click);
        }
        internal void Reset()
        {
            lblTotalPageCount.Text = "0";
            txtCurrentPageIndex.Text = "1";
            cmbCurrentZoomLevel.IsEnabled = false;
            btnFitPage.IsEnabled = false;
            btnFitWidth.IsEnabled = false;
            btnZoomIn.IsEnabled = false;
            btnZoomOut.IsEnabled = false;
            m_activeView.LoadedDocument = null;
        }

        #region Properties
        public PdfDocumentView ActiveView
        {
            get
            {
                return m_activeView;
            }
            set
            {
                if (m_activeView != value)
                {
                    m_activeView = value;
                    Initialize(value);
                }
            }
        }
        #endregion

        #region Implementation
        public void Initialize(PdfDocumentView view)
        {
            UnWireEvents();
            WireUpEvents();
            m_activeView = view;
            if (m_activeView.LoadedDocument == null)
                this.cmbCurrentZoomLevel.IsEnabled = false;
            else
                this.cmbCurrentZoomLevel.IsEnabled = true;
            m_activeView.PdfDocumentViewer.NavigationButtonStatesChanged += new NavigationButtonStatesChangedEventHandler(m_activeView_NavigationButtonStatesChanged);
            m_activeView.PdfDocumentViewer.CurrentPageChanged += new CurrentPageChangedEventHandler(m_activeView_CurrentPageChanged);

            txtCurrentPageIndex.Text = (m_activeView.PdfDocumentViewer.CurrentPageIndex + 1).ToString();
            lblTotalPageCount.Text = m_activeView.PageCount.ToString();

            
            m_activeView_NavigationButtonStatesChanged(null, null);

            m_activeView.ZoomChanged += new PdfDocumentView.ZoomChangedEventHandler(m_activeView_ZoomChanged);
            cmbCurrentZoomLevel.SelectedItem = cmbCurrentZoomLevel.Items[4];
        }

        void m_activeView_ZoomChanged(object sender, ZoomEventArgs args)
        {
            cmbCurrentZoomLevel.Text = string.Format("{0}%", args.ZoomPercentage);
        }

        void m_activeView_CurrentPageChanged(object sender, EventArgs args)
        {
            SetCurrentPageIndex(m_activeView.PdfDocumentViewer.CurrentPageIndex + 1);
        }

        void m_activeView_NavigationButtonStatesChanged(object sender, EventArgs args)
        {
            btnGoToFirstPage.IsEnabled = m_activeView.PdfDocumentViewer.CanGoToFirstPage;
            btnGoToLastPage.IsEnabled = m_activeView.PdfDocumentViewer.CanGoToLastPage;
            btnGoToNextPage.IsEnabled = m_activeView.PdfDocumentViewer.CanGoToNextPage;
            btnGoToPreviousPage.IsEnabled = m_activeView.PdfDocumentViewer.CanGoToPreviousPage;
        }

        void SetCurrentPageIndex(int index)
        {
            txtCurrentPageIndex.Text = index.ToString();
        }
        #endregion


        #region Helper Methods
        void WireUpEvents()
        {
            btnSave.Click += new RoutedEventHandler(btnSave_Click);

            btnGoToFirstPage.Click += new RoutedEventHandler(btnGoToFirstPage_Click);
            btnGoToPreviousPage.Click += new RoutedEventHandler(btnGoToPreviousPage_Click);
            btnGoToNextPage.Click += new RoutedEventHandler(btnGoToNextPage_Click);
            btnGoToLastPage.Click += new RoutedEventHandler(btnGoToLastPage_Click);
            btnPrint.Click += new RoutedEventHandler(btnPrint_Click);
            txtCurrentPageIndex.KeyDown += new KeyEventHandler(txtCurrentPageIndex_KeyDown);

            btnGoToFirstPage.IsEnabledChanged += new DependencyPropertyChangedEventHandler(btnGoToFirstPage_EnabledChanged);
            btnGoToLastPage.IsEnabledChanged += new DependencyPropertyChangedEventHandler(btnGoToLastPage_EnabledChanged);
            btnGoToNextPage.IsEnabledChanged += new DependencyPropertyChangedEventHandler(btnGoToNextPage_EnabledChanged);
            btnGoToPreviousPage.IsEnabledChanged += new DependencyPropertyChangedEventHandler(btnGoToPreviousPage_EnabledChanged);

            btnFitPage.Click += new RoutedEventHandler(btnFitPage_Click);
            btnFitWidth.Click += new RoutedEventHandler(btnFitWidth_Click);
            btnZoomIn.Click += new RoutedEventHandler(btnZoomIn_Click);
            btnZoomOut.Click += new RoutedEventHandler(btnZoomOut_Click);

            cmbCurrentZoomLevel.SelectionChanged += new SelectionChangedEventHandler(cmbCurrentZoomLevel_SelectionChanged);
            cmbCurrentZoomLevel.KeyDown += new KeyEventHandler(cmbCurrentZoomLevel_KeyDown);
        }

        void cmbCurrentZoomLevel_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox zoomBox = sender as ComboBox;
            if (e.Key == Key.Enter)
            {
                string zoomEntered = zoomBox.Text;
                int magnificationValue;
                if (zoomEntered.Contains("%"))
                {
                    int index = zoomEntered.IndexOf('%');
                    zoomEntered = zoomEntered.Substring(0, index);
                }
                int.TryParse(zoomEntered, out magnificationValue);
                if (magnificationValue < 10)
                    magnificationValue = 10;
                if (magnificationValue > 6400)
                    magnificationValue = 6400;

                m_activeView.ZoomTo(magnificationValue);
                cmbCurrentZoomLevel.Text = magnificationValue.ToString() + "%";
            }
        }

        void UnWireEvents()
        {
            btnSave.Click -= new RoutedEventHandler(btnSave_Click);

            btnGoToFirstPage.Click -= new RoutedEventHandler(btnGoToFirstPage_Click);
            btnGoToPreviousPage.Click -= new RoutedEventHandler(btnGoToPreviousPage_Click);
            btnGoToNextPage.Click -= new RoutedEventHandler(btnGoToNextPage_Click);
            btnGoToLastPage.Click -= new RoutedEventHandler(btnGoToLastPage_Click);
            btnPrint.Click -= new RoutedEventHandler(btnPrint_Click);
            txtCurrentPageIndex.KeyDown -= new KeyEventHandler(txtCurrentPageIndex_KeyDown);

            btnGoToFirstPage.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(btnGoToFirstPage_EnabledChanged);
            btnGoToLastPage.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(btnGoToLastPage_EnabledChanged);
            btnGoToNextPage.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(btnGoToNextPage_EnabledChanged);
            btnGoToPreviousPage.IsEnabledChanged -= new DependencyPropertyChangedEventHandler(btnGoToPreviousPage_EnabledChanged);

            btnFitPage.Click -= new RoutedEventHandler(btnFitPage_Click);
            btnFitWidth.Click -= new RoutedEventHandler(btnFitWidth_Click);
            btnZoomIn.Click -= new RoutedEventHandler(btnZoomIn_Click);
            btnZoomOut.Click -= new RoutedEventHandler(btnZoomOut_Click);

            cmbCurrentZoomLevel.SelectionChanged -= new SelectionChangedEventHandler(cmbCurrentZoomLevel_SelectionChanged);
            cmbCurrentZoomLevel.KeyDown -= new KeyEventHandler(cmbCurrentZoomLevel_KeyDown);
        }
        void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files *.pdf|*.pdf";
            openFileDialog.Title = "Select a file";
            exceptions.Exceptions.Remove(0, exceptions.Exceptions.Length);
            if (openFileDialog.ShowDialog().Value)
            {
                m_activeView.Unload();
                FileInfo fi = new FileInfo(openFileDialog.FileName);
                filename = fi.Name;
                try
                {
                    m_activeView.Load(openFileDialog.FileName);
                }
                catch (Syncfusion.Pdf.PdfDocumentException)
                {
                    PasswordToolBar dlg = new PasswordToolBar();
                    dlg.Owner = Window.GetWindow(this);
                    dlg.ShowDialog();

                    if (dlg.DialogResult == true)
                    {
                        try
                        {
                            m_activeView.Load(openFileDialog.FileName, dlg.Password);
                        }
                        catch (Syncfusion.Pdf.PdfDocumentException)
                        {
                            MessageBox.Show("Password is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                Initialize(m_activeView);
                this.lblTotalPageCount.Text = m_activeView.PageCount.ToString();
                this.cmbCurrentZoomLevel.IsEnabled = true;
                this.cmbCurrentZoomLevel.Text = "100%";
            }
        }

        void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (m_activeView != null)
            {
                if (m_activeView.LoadedDocument != null)
                {
                    PdfLoadedDocument ldoc = m_activeView.LoadedDocument;
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "PDF Files (*.pdf)|*.pdf";
                    if (filename != string.Empty)
                        save.FileName = filename;
                    else if (m_activeView.FileName != null)
                    {
                        FileInfo fi = new FileInfo(m_activeView.FileName);
                        save.FileName = fi.Name;
                    }
                    if (save.ShowDialog() == true)
                    {
                        if (save.FileName != string.Empty)
                            ldoc.Save(save.FileName);
                    }
                }
            }
        }

        void cmbCurrentZoomLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).IsDropDownOpen)
            {
                int zoomLevel = GetCurrentZoomLevel();
                m_activeView.ZoomTo(zoomLevel);
            }
        }

        void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_activeView.LoadedDocument == null)
            {
                return;
            }
            int currentZoomLevel = GetCurrentZoomLevel();
            int index = GetComboBoxItemIndex(string.Format("{0}%", currentZoomLevel));
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
                            cmbCurrentZoomLevel.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            string zoomEntered = cmbCurrentZoomLevel.Text;
            int magnificationValue;
            if (zoomEntered.Contains("%"))
            {
                int index1 = zoomEntered.IndexOf('%');
                zoomEntered = zoomEntered.Substring(0, index1);
            }
            int.TryParse(zoomEntered, out magnificationValue);
            if (magnificationValue < 10)
                magnificationValue = 10;
            if (magnificationValue > 6400)
                magnificationValue = 6400;

            m_activeView.ZoomTo(magnificationValue);
        }

        void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (this.m_activeView.LoadedDocument == null)
            {
                return;
            }
            int currentZoomLevel = GetCurrentZoomLevel();
            int index = GetComboBoxItemIndex(string.Format("{0}%", currentZoomLevel));
            if (index == 0||(index - 1 >= 0 && index + 1 < cmbCurrentZoomLevel.Items.Count))
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
                            break;
                        }
                    }
                }
            }

            string zoomEntered = cmbCurrentZoomLevel.Text;
            int magnificationValue;
            if (zoomEntered.Contains("%"))
            {
                int index1 = zoomEntered.IndexOf('%');
                zoomEntered = zoomEntered.Substring(0, index1);
            }
            int.TryParse(zoomEntered, out magnificationValue);
            if (magnificationValue < 10)
                magnificationValue = 10;
            if (magnificationValue > 6400)
                magnificationValue = 6400;

            m_activeView.ZoomTo(magnificationValue);
        }

        int GetComboBoxItemIndex(string text)
        {
            for(int i=0; i<cmbCurrentZoomLevel.Items.Count; i++)
            {
                if(String.Equals(
                    (cmbCurrentZoomLevel.Items[i] as ComboBoxItem).Content, text))
                    return i;
            }

            return -1;
        }

        int GetCurrentZoomLevel()
        {
            string text = "";
            if (cmbCurrentZoomLevel.SelectedValue == null)
                text = cmbCurrentZoomLevel.Text;
            else
                 text = (cmbCurrentZoomLevel.SelectedValue as ComboBoxItem)
                .Content.ToString();

            text = text.TrimEnd(new char[] { '%' });
            int zoomLevel = 100;

            if (int.TryParse(text, out zoomLevel))
            {
                return zoomLevel;
            }

            return 100;
        }

        void btnFitWidth_Click(object sender, RoutedEventArgs e)
        {
            m_activeView.ZoomMode = ZoomMode.FitWidth;
        }

        void btnFitPage_Click(object sender, RoutedEventArgs e)
        {
            m_activeView.ZoomMode = ZoomMode.FitPage;
        }

        void btnGoToPreviousPage_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Image buttonImage = btnGoToPreviousPage.Content as Image;
            buttonImage.Source =
                (btnGoToPreviousPage.IsEnabled)
                    ? this.Resources["GoToPreviousPage_Enabled"] as ImageSource
                    : this.Resources["GoToPreviousPage_Disabled"] as ImageSource;
        }

        void btnGoToNextPage_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Image buttonImage = btnGoToNextPage.Content as Image;
            buttonImage.Source =
                (btnGoToNextPage.IsEnabled)
                    ? this.Resources["GoToNextPage_Enabled"] as ImageSource
                    : this.Resources["GoToNextPage_Disabled"] as ImageSource;
        }

        void btnGoToLastPage_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Image buttonImage = btnGoToLastPage.Content as Image;
            buttonImage.Source=
                (btnGoToLastPage.IsEnabled)
                    ? this.Resources["GoToLastPage_Enabled"] as ImageSource
                    : this.Resources["GoToLastPage_Disabled"] as ImageSource;
        }

        void btnGoToFirstPage_EnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Image buttonImage = btnGoToFirstPage.Content as Image;
            buttonImage.Source =
                (btnGoToFirstPage.IsEnabled)
                    ? this.Resources["GoToFirstPage_Enabled"] as ImageSource
                    : this.Resources["GoToFirstPage_Disabled"] as ImageSource;
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
            if (e.Key == Key.Enter && m_activeView.LoadedDocument != null)
            {
                m_activeView.GoToPageAtIndex(
                    GetCurrentPageIndex());
            }
        }


        void btnPrint_Click(object sender, EventArgs e)
        {
            Print();
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

        void Print()
        {
            if (m_activeView.LoadedDocument != null)
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.UserPageRangeEnabled = true;
                if (printDialog.ShowDialog() == true)
                {
                    DocumentPaginator paginator = m_activeView.PdfDocumentViewer.PrintDocument.DocumentPaginator;
                    if (printDialog.PageRangeSelection == PageRangeSelection.UserPages)
                        paginator = new PageRangeDocumentPaginator(paginator, printDialog.PageRange);
                    printDialog.PrintDocument(paginator, "das");
                }
            }
        }     
        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);
            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
            if (e.ChangedButton == MouseButton.Right)
            {
                e.Handled = true;
            }
        }
        #endregion
       
    }
    
    internal class PageRangeDocumentPaginator : DocumentPaginator
    {
        private int startIndex;
        private int endIndex;
        private DocumentPaginator paginator;
        public PageRangeDocumentPaginator(DocumentPaginator documentpaginator, PageRange pageRange)
        {
            startIndex = pageRange.PageFrom - 1;
            endIndex = pageRange.PageTo - 1;
            paginator = documentpaginator;
            endIndex = Math.Min(endIndex, documentpaginator.PageCount - 1);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            var page = paginator.GetPage(pageNumber + startIndex);
            var cv = new ContainerVisual();
            if (page.Visual is FixedPage)
            {
                foreach (var child in ((FixedPage)page.Visual).Children)
                {
                    var childClone = (UIElement)child.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(child, null);
                    var parentField = childClone.GetType().GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (parentField != null)
                    {
                        parentField.SetValue(childClone, null);
                        cv.Children.Add(childClone);
                    }
                }
                return new DocumentPage(cv, page.Size, page.BleedBox, page.ContentBox);
            }
            return page;
        }

        public override bool IsPageCountValid
        {
            get 
            { 
                return true; 
            }
        }

        public override int PageCount
        {
            get
            {
                if (startIndex > paginator.PageCount - 1)
                    return 0;
                if (startIndex > endIndex)
                    return 0;
                return endIndex - startIndex + 1;
            }
        }

        public override Size PageSize
        {
            get 
            { 
                return paginator.PageSize; 
            }
            set 
            { 
                paginator.PageSize = value; 
            }
        }

        public override IDocumentPaginatorSource Source
        {
            get 
            { 
                return paginator.Source; 
            }
        }
    }
}
