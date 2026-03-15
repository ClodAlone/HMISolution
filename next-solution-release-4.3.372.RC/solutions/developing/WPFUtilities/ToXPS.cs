using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace WPFUtilities
{
    public class ToXPS
    {
        void ToFile(FrameworkElement fe)
        {
            // Initialize the xps document structure
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            //Create first page of document
            fixedPage.Children.Add(fe);
            fixedPage.Width = fe.ActualWidth;
            fixedPage.Height = fe.ActualHeight;

            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);


            Size sz = new Size(fixedPage.Width, fixedPage.Height);
            fixedPage.Measure(sz);
            fixedPage.Arrange(new Rect(new Point(), sz));
            fixedPage.UpdateLayout();

            var fileName = @"C:\Temp\test.xps";
            try
            {
                File.Delete(fileName);
            }
            catch
            {

            }
            // Create the xps file and write it
            XpsDocument xpsd = new XpsDocument(fileName, FileAccess.ReadWrite);
            XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            xw.Write(fixedDoc);
            xpsd.Close();

            fixedPage.Children.Remove(fe);
        }
    }
}
