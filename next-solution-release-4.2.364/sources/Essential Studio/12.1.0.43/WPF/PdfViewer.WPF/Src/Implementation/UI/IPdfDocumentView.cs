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
using System.Windows.Controls;
using Syncfusion.Pdf.Parsing;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace Syncfusion.Windows.PdfViewer
{
    public delegate void CurrentPageChangedEventHandler(object sender, EventArgs args);
    public delegate void NavigationButtonStatesChangedEventHandler(object sender, EventArgs args);
    internal interface IPdfDocumentView
    {
        ScrollViewer ScrollViewer { get; }
        UserControl Instance { get; }
        void Load(string filePath);
        void Load(Stream stream);
        void Load(String filePath, string password);
        void Load(PdfLoadedDocument loadedDocument);
        void Unload();
        void ZoomTo(double zoomFactor);
        double ZoomFactor {get; set;}
        //void ZoomTo(double zoomLevel);
        void ZoomIn();
        void SetZoom();
        void GotoNextPage();
        void GotoPreviousPage();
        void ZoomOut();
        void UpdateOffset(double offSet);
        int m_nextMatch { get; set; }
        int CurrentPageIndex { get; set; }
        int PageCount { get; }
        void GoToPageAtIndex(int pageIndex);
        PdfLoadedDocument LoadedDocument { get; set; }
        event CurrentPageChangedEventHandler CurrentPageChanged;
        event NavigationButtonStatesChangedEventHandler NavigationButtonStatesChanged;
        bool CanGoToFirstPage { get; set; }
        bool CanGoToPreviousPage { get; set; }
        bool CanGoToNextPage { get; set; }
        bool CanGoToLastPage { get; set; }
        bool ShowPageNumber { get; set; }
        FixedDocument PrintDocument{get;}
        void Print();
        void ClearSearch();
        void TextSearch(String txt, bool IsNextClicked);
        BitmapSource DisplaybyAddingControl(int pageindex);
        BitmapSource ExportAsImage(int pageIndex);
        BitmapSource[] ExportAsImage(int startIndex, int endIndex);
        void DrawTextSearch(int i);
    }
}
