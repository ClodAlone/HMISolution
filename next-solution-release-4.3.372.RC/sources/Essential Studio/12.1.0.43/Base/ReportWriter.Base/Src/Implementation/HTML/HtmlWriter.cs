#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Drawing;
#if ! SILVERLIGHT
using System.Drawing.Imaging;
#endif
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Syncfusion.RDL.Layout;
using Syncfusion.RDL.Data;
using Syncfusion.RDL.Internal;
using Syncfusion.RDL.ItemModel;
using System.Threading;

#if !WINRT 
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
using Syncfusion.XlsIO;
#endif

namespace Syncfusion.ReportWriter
{
    class HtmlWriter : WriterBase
    {
        #region Fields

        private int m_CurrentPage = 0;
        private int m_totalPages = 0;
        private int m_styleCount = 0;
        private bool isHeaderFooter;
        private string cssClasses;
        private string styleKey;
        private MemoryStream m_Headerstream;
        private MemoryStream m_FooterStream;
#if SILVERLIGHT
        private XmlWriter m_writer;
#else
        private XmlTextWriter m_writer;
#endif
        private Dictionary<string, MemoryStream> imageValues;
        private Dictionary<string, string> styleValues;
        private Dictionary<int, MemoryStream> bodyContents;

        #endregion

        #region public Methods
#if ! SILVERLIGHT

        public void Save(string fileName, System.Web.HttpResponse response)
        {
            if (this.ReportModel.HasReport)
            {
                Thread thread = new Thread(delegate()
                {
                    response.ClearContent();
                    response.Expires = 0;
                    response.Buffer = true;
                    response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    response.AddHeader("Content-Type", "text/html");
                    response.Clear();
                    Save(response.OutputStream);
                    response.Flush();
                    response.End();
                });

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                thread.Join();
            }
            else
            {
                throw new Exception("Load the Report for PDF Writer");
            }
        }
#endif
#if !WINRT 

        /// <summary>
        /// Exports the report as a Html document
        /// </summary>
        /// <param name="HtmlFilename">The name of the html file to be saved</param>
        public void Save(string HtmlFilename)
        {
            using (FileStream htmlStream = new FileStream(HtmlFilename, FileMode.Create))
            {
                Save(htmlStream);
            }
        }
#endif
        /// <summary>
        /// Exports the report as a html document
        /// </summary>
        /// <param name="htmlStream">The stream where the html document to be saved</param>
        /// <remarks></remarks>
        public void Save(Stream htmlStream)
        {
            if (this.ReportModel.HasReport)
            {
                PageModelFactory pageModelFactory = UpdatePageLayoutForHTML(ReportModel);
                this.ConvertToHTML(pageModelFactory, ReportModel);
#if SILVERLIGHT
                this.m_writer = XmlWriter.Create(htmlStream);
#else
                this.m_writer = new XmlTextWriter(htmlStream, Encoding.UTF8);
#endif
                this.WriteHtml();
                htmlStream.Flush();
                this.ResetValues();
            }
            else
            {
                throw new Exception("Load the Report for Html Writer");
            }
        }

        private void ResetValues()
        {
            this.m_writer.Flush();
#if !WINRT
            this.m_writer.Close();
#endif

            this.m_writer = null;

            this.imageValues.Clear();
            this.imageValues = null;

            this.styleValues.Clear();
            this.styleValues = null;
        }

        #endregion

        #region helper methods

        private PageModelFactory UpdatePageLayoutForHTML(ReportModel reportModel)
        {
            if (!this.ReportModel.IsEvaluatedReport)
            {
                if (this.DataSources.Count == 0)
                {
                    reportModel.InitilizeReport();
                }
                else
                {
                    reportModel.IsRDLC = true;
                    reportModel.DataSources = this.DataSources;
                    reportModel.InitilizeReport();
                }

                this.ReportModel.Evaluate();
                this.ReportModel.UpdateSize();
            }

            PageModelFactory pageModelFactory = new PageModelFactory(this.ReportModel);
            pageModelFactory.UpdatePageLayout();

            return pageModelFactory;
        }

        private void ConvertToHTML(PageModelFactory pageModelFactory, ReportModel reportModel)
        {
            this.bodyContents = new Dictionary<int, MemoryStream>();
            this.styleValues = new Dictionary<string, string>();
            this.imageValues = new Dictionary<string, MemoryStream>();

            string pageWidth = this.GetPixelString(this.ReportDefinition.Width.PixelValue);
            this.m_totalPages = pageModelFactory.PageDictionary.Count();
            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();

            if (pageModelFactory.HeaderHeight > 0)
            {
                m_Headerstream = new MemoryStream();
#if SILVERLIGHT
                XmlWriter headwriter = XmlWriter.Create(m_Headerstream);
#else
                XmlTextWriter headwriter = new XmlTextWriter(m_Headerstream, Encoding.UTF8);
#endif
                headwriter.WriteStartElement("DIV");
                if (pageModelFactory.Model.HeaderReportItemModels.Count != 0)
                {
                    ProcessHeaderFooteritems(pageModelFactory.Model.HeaderReportItemModels, headwriter);
                }
                headwriter.WriteEndElement();
                styleKey = "position:relative; height:" + this.GetPixelString(pageModelFactory.HeaderHeight) + ";" + "width:" + pageWidth + ";";
                cssClasses = GetCssClass(styleKey);

                if (!string.IsNullOrEmpty(reportModel.HeaderBehaviour.ImageValue))
                {
                    cssClasses += " " + GetCssClass(GetBackgroundImage(reportModel.HeaderBehaviour.ImageValue));
                }

                cssClasses += " " + GetCssClass("background-color:" + reportModel.HeaderBehaviour.BackgroudColor + ";");
                cssClasses += " " + this.GetBorderStyles(reportModel.HeaderBehaviour.Border);
                this.styleValues.Add("header", cssClasses);
                headwriter.Flush();
            }

            if (pageModelFactory.FooterHeight > 0)
            {
                m_FooterStream = new MemoryStream();
#if SILVERLIGHT
                var footerwriter = XmlWriter.Create(m_FooterStream);
#else
                XmlTextWriter footerwriter = new XmlTextWriter(m_FooterStream, Encoding.UTF8);
#endif
                footerwriter.WriteStartElement("DIV");

                if (pageModelFactory.Model.FooterReportItemModels.Count != 0)
                {
                    ProcessHeaderFooteritems(pageModelFactory.Model.FooterReportItemModels, footerwriter);
                }
                footerwriter.WriteEndElement();

                styleKey = "position:relative;height:" + this.GetPixelString(pageModelFactory.FooterHeight) + ";" + "width:" + pageWidth + ";";
                cssClasses = GetCssClass(styleKey);

                if (!string.IsNullOrEmpty(reportModel.FooterBehaviour.ImageValue))
                {
                    cssClasses += " " + GetCssClass(GetBackgroundImage(reportModel.FooterBehaviour.ImageValue));
                }

                cssClasses += " " + GetCssClass("background-color:" + reportModel.FooterBehaviour.BackgroudColor + ";");
                cssClasses += " " + this.GetBorderStyles(reportModel.FooterBehaviour.Border);
                this.styleValues.Add("footer", cssClasses);
                footerwriter.Flush();
            }

            string pageStyle = GetCssClass("background-color:" + reportModel.BodyBehaviour.BackgroudColor + ";");
            pageStyle += " " + this.GetBorderStyles(reportModel.BodyBehaviour.Border);

            if (!string.IsNullOrEmpty(reportModel.BodyBehaviour.ImageValue))
            {
                cssClasses += " " + GetCssClass(GetBackgroundImage(reportModel.BodyBehaviour.ImageValue));
            }

            foreach (var pageModel in pageModelFactory.PageDictionary)
            {
                MemoryStream bodystream = new MemoryStream();
#if SILVERLIGHT
                XmlWriter writer = XmlWriter.Create(bodystream);
#else
                XmlTextWriter writer = new XmlTextWriter(bodystream, Encoding.UTF8);
#endif
                writer.WriteStartElement("DIV");
                writer.WriteAttributeString("class", pageStyle);
                writer.WriteAttributeString("style", "position:relative;height:" + this.GetPixelString(pageModel.Value.Height) + ";" + "width:" + pageWidth + ";");

                foreach (var item in pageModel.Value.ReportModelCollection)
                {
                    ProcessReportModel(item, writer);
                }

                writer.WriteEndElement();
                writer.Flush();
                this.bodyContents.Add(this.m_CurrentPage, bodystream);
                this.m_CurrentPage++;
            }
        }

#if SILVERLIGHT
        private void ProcessHeaderFooteritems(ReportModelContentCollection reportModelContentCollection, XmlWriter writer)
#else
        private void ProcessHeaderFooteritems(ReportModelContentCollection reportModelContentCollection, XmlTextWriter writer)
#endif
        {
            foreach (var reportModel in reportModelContentCollection)
            {
                this.isHeaderFooter = true;

                switch (reportModel.ModelType)
                {
                    case ModelType.TextBoxModel:
                        ProcessTextBox(reportModel, writer);
                        break;
                    case ModelType.LineModel:
                        ProcessLineModel(reportModel, writer);
                        break;
                    case ModelType.ImageModel:
                        ProcessImageModel(reportModel, writer);
                        break;
                    case ModelType.RectangleModel:
                        ProcessRectangleModel(reportModel, writer);
                        break;
                }
            }
        }

#if SILVERLIGHT
        private void ProcessReportModel(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessReportModel(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            this.isHeaderFooter = false;
            switch (reportModel.ModelType)
            {
                case ModelType.TextBoxModel:
                    ProcessTextBox(reportModel, writer);
                    break;
                case ModelType.ImageModel:
                    ProcessImageModel(reportModel, writer);
                    break;
                case ModelType.LineModel:
                    ProcessLineModel(reportModel, writer);
                    break;
                case ModelType.RectangleModel:
                    ProcessRectangleModel(reportModel, writer);
                    break;
#if ! SILVERLIGHT
                case ModelType.GaugeModel:
                    ProcessGaugeModel(reportModel, writer);
                    break;
                case ModelType.ChartModel:
                    ProcessChartModel(reportModel, writer);
                    break;
#endif
                case ModelType.TablixModel:
                    ProcessTablixModel(reportModel, writer);
                    break;
#if ! SILVERLIGHT
#if !SyncfusionFramework3_5
                case ModelType.MapModel:
                    ProcessMapModel(reportModel, writer);
                    break;
#endif
#endif

            }
        }

#if SILVERLIGHT
        private void ProcessTextBox(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessTextBox(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            try
            {
                TextboxModel textbox = (TextboxModel)reportModel;
                bool hasLink = false;
                styleKey = "position:absolute;";

                if (textbox.IsTablixChild && textbox.IsTablixInnerChild)
                {
                    styleKey += "top:" + GetPixelString(textbox.Top) + ";left:" + GetPixelString(textbox.Left) + ";";
                }
                else if (textbox.PageInfo != null)
                {
                    styleKey += "top:" + GetPixelString(textbox.PageInfo.ActualTop) + ";left:" + GetPixelString(textbox.PageInfo.ActualLeft) + ";";
                }

                styleKey += "height:" + GetPixelString(textbox.Height) + ";" + "width:" + GetPixelString(textbox.Width) + ";";
                cssClasses = GetCssClass(styleKey);
                cssClasses += " " + GetCssClass("background-color:" + textbox.TextBoxProperties.BackGroundColor + ";");
                cssClasses += " " + GetCssClass("vertical-align:" + textbox.TextBoxProperties.VerticalAlignment + ";");

                if (textbox.TextBoxProperties.Border != null)
                {
                    cssClasses += " " + this.GetBorderStyles(textbox.TextBoxProperties.Border);
                }
                if (textbox.ReportItem != null && textbox.ReportItem.Style.BackgroundImage != null)
                {
                    cssClasses += " " + GetCssClass(GetBackgroundImage(textbox.ReportItem.Style.BackgroundImage.Value));
                }

                if (textbox.ReportItem != null && textbox.ReportItem.ActionInfo != null)
                {
                    var action = textbox.ReportItem.ActionInfo.Actions.First();
                    writer.WriteStartElement("a");
                    this.GetHyperLink(action, writer);
                    hasLink = true;
                }

                writer.WriteStartElement("DIV");
                writer.WriteAttributeString("class", cssClasses);
                char i = 'a';

                foreach (var block in textbox.ParaExpval)
                {
                    if (block.Runs.Count != 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var runs in block.Runs)
                        {
                            cssClasses = this.GetTextRunStyle(runs);
                            sb.AppendFormat("<SPAN class=\"{0}\">", cssClasses);
                            if (this.isHeaderFooter)
                                sb.Append(this.ProcessRunText(runs.Text));
                            else
                                sb.Append(runs.Text);

                            sb.Append("</SPAN>");
                            i++;
                        }

                        cssClasses = "";

                        if (textbox.TextBoxProperties.Padding != null)
                        {
                            cssClasses = GetPaddingCss(textbox.TextBoxProperties.Padding);
                        }

                        writer.WriteStartElement("DIV");
                        writer.WriteAttributeString("class", cssClasses + " " + GetCssClass("text-align:" + block.TextAlignment + ";"));
                        writer.WriteRaw(sb.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteRaw("");
                writer.WriteEndElement();
                if (hasLink)
                {
                    writer.WriteEndElement();
                }
                writer.WriteRaw("\n");
            }
            catch
            {

            }
        }

#if SILVERLIGHT
        private void ProcessLineModel(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessLineModel(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            LineModel lineModel = (LineModel)reportModel;
            int width = Convert.ToInt32(lineModel.Width);
            int height = Convert.ToInt32(lineModel.Height);
            styleKey = "position:absolute;width:" + width + "px;height:" + height + "px;";

            if (lineModel.PageInfo != null)
            {
                styleKey += "top:" + GetPixelString(lineModel.PageInfo.ActualTop) + ";left:" + GetPixelString(lineModel.PageInfo.ActualLeft) + ";";
            }
            else
            {
                styleKey += "top:" + GetPixelString(lineModel.Top) + ";left:" + GetPixelString(lineModel.Left) + ";";
            }

            cssClasses = GetCssClass(styleKey);
            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", cssClasses);
            string lineWidth = this.GetPixelString(lineModel.LineProperties.LineWidth == 0 ? 1 : lineModel.LineProperties.LineWidth);

            if (lineModel.ReportItem.Height.FloatValue == 0)
            {
                writer.WriteRaw(string.Format("<hr width=\"{0}px\" ", width));
                writer.WriteRaw(string.Format("style=\"border:0px solid white; border-top:{0} {1} {2};\" />", lineWidth, lineModel.LineProperties.LineStyle, lineModel.LineProperties.LineColor));
            }
            else if (lineModel.ReportItem.Width.FloatValue == 0)
            {
                writer.WriteRaw(string.Format("<hr size=\"{0}px\" ", height));
                writer.WriteRaw(string.Format("style=\"border:0px solid white; border-left:{0} {1} {2};\" />", lineWidth, lineModel.LineProperties.LineStyle, lineModel.LineProperties.LineColor));
            }
            else
            {
                writer.WriteRaw("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
                writer.WriteRaw("<v:group coordsize=\"100,100\" coordorigin=\"0,0\"");
                writer.WriteRaw(string.Format("style=\"WIDTH:{0}px;HEIGHT:{1}px;\">", width, height));

                if (lineModel.ReportItem.Height.FloatValue < 0)
                {
                    writer.WriteRaw("<v:line from=\"0,100\" to=\"100,0\"");
                }
                else
                {
                    writer.WriteRaw("<v:line from=\"0,0\" to=\"100,100\"");
                }

                writer.WriteRaw(string.Format(" strokecolor=\"{0}\" strokeWeight=\"{1}\"><v:stroke dashstyle=\"{2}\"/>", lineModel.LineProperties.LineColor, lineWidth, lineModel.LineProperties.LineStyle));
                writer.WriteRaw("</v:line></v:group>");
            }
            writer.WriteEndElement();
            writer.WriteRaw("\n");
        }

#if !SILVERLIGHT
        private void ProcessChartModel(IReportItemModeler reportModel, XmlTextWriter writer)
        {
            ChartModel chartModel = (ChartModel)reportModel;
            System.Drawing.Image chartimage = System.Drawing.Image.FromStream(chartModel.GetImageStream());
            int width = Convert.ToInt32(chartModel.Width);
            int height = Convert.ToInt32(chartModel.Height);
            Bitmap image = new Bitmap(chartimage, width, height);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            styleKey = "position:absolute;width:" + width + "px;height:" + height + "px;";

            if (chartModel.PageInfo != null)
            {
                styleKey += "top:" + GetPixelString(chartModel.PageInfo.ActualTop) + ";left:" + GetPixelString(chartModel.PageInfo.ActualLeft) + ";";
            }
            else
            {
                styleKey += "top:" + GetPixelString(chartModel.Top) + ";left:" + GetPixelString(chartModel.Left) + ";";
            }

            cssClasses = GetCssClass(styleKey);

            if (chartModel.ChartProperties.Border != null && chartModel.ChartProperties.Border.Default != null)
            {
                cssClasses += " " + GetBorderStyles(chartModel.ChartProperties.Border);
            }

            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", cssClasses);
            writer.WriteStartElement("IMG");
            string str3 = "data:image/jpeg";
            string imgstream = Convert.ToBase64String(stream.ToArray());
            writer.WriteAttributeString("src", str3 + ";base64," + imgstream);
            writer.WriteAttributeString("alt", chartModel.Name);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteRaw("\n");
        }

        private void ProcessGaugeModel(IReportItemModeler reportModel, XmlTextWriter writer)
        {
            GaugeModel gaugeModel = (GaugeModel)reportModel;
            System.Drawing.Image gaugeimage = System.Drawing.Image.FromStream(gaugeModel.GetImageStream());
            int width = Convert.ToInt32(gaugeModel.Width);
            int height = Convert.ToInt32(gaugeModel.Height);
            Bitmap image = new Bitmap(gaugeimage, width, height);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            styleKey = "position:absolute;width:" + width + "px;height:" + height + "px;";

            if (gaugeModel.PageInfo != null)
            {
                styleKey += "top:" + GetPixelString(gaugeModel.PageInfo.ActualTop) + ";left:" + GetPixelString(gaugeModel.PageInfo.ActualLeft) + ";";
            }
            else
            {
                styleKey += "top:" + GetPixelString(gaugeModel.Top) + ";left:" + GetPixelString(gaugeModel.Left) + ";";
            }

            cssClasses = GetCssClass(styleKey);

            if (gaugeModel.GaugePanelProperties.Border != null && gaugeModel.GaugePanelProperties.Border.Default != null)
            {
                double thickness = gaugeModel.GaugePanelProperties.Border.Default.Thickness == 0.0 ? 1 : gaugeModel.GaugePanelProperties.Border.Default.Thickness;
                styleKey = "border:" + thickness + "px " + gaugeModel.GaugePanelProperties.Border.Default.BorderStyle +
                    " " + gaugeModel.GaugePanelProperties.Border.Default.BorderBrush + ";";
                cssClasses += " " + GetCssClass(styleKey);
            }

            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", cssClasses);
            writer.WriteStartElement("IMG");
            string str3 = "data:image/jpeg";
            string imgstream = Convert.ToBase64String(stream.ToArray());
            writer.WriteAttributeString("src", str3 + ";base64," + imgstream);
            writer.WriteAttributeString("alt", gaugeModel.Name);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteRaw("\n");
        }

#endif

#if SILVERLIGHT
        private void ProcessImageModel(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessImageModel(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            try
            {
                ImageModel imageModel = reportModel as ImageModel;
                object imgData = imageModel.ImageData;
                int width = Convert.ToInt32(imageModel.Width);
                int height = Convert.ToInt32(imageModel.Height);
                Base64ImageConverter base64ImageConverter = new Base64ImageConverter();

#if ! SILVERLIGHT
#if !WINRT
            Stream imgStream = null;
#endif
            try
            {
                imgStream = ((BitmapImage)base64ImageConverter.ConvertToImage(imageModel.ImageData)).StreamSource;
            }
            catch
            {
                imgStream = ((BitmapImage)BufferImage(imageModel.ImageData)).StreamSource;
            }

            Image original = Image.FromStream(imgStream);
            Bitmap image = null;

            ImageFormat format = this.GetImageFormat(imageModel.ImageFormat.ToString());
            if (format == ImageFormat.Wmf)
            {
                Metafile metafile = original as Metafile;
                GraphicsUnit display = GraphicsUnit.Display;
#if SIVERLIGHT
                Size size =new Size(metafile.GetBounds(ref display).Size);
#else
                Size size = Size.Ceiling(metafile.GetBounds(ref display).Size);
#endif
                image = new Bitmap(size.Width, size.Height);
                image.SetResolution(metafile.HorizontalResolution, metafile.VerticalResolution);
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    graphics.DrawImageUnscaled(metafile, Point.Empty);
                    graphics.Dispose();
                }
            }


            else
            {
                image = new Bitmap(original, width, height);
            }
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
#else
                MemoryStream stream = new MemoryStream(imgData as byte[]);

#endif

                styleKey = "position:absolute;width:" + width + "px;height:" + height + "px;";

                if (imageModel.IsTablixChild && imageModel.IsTablixInnerChild)
                {
                    styleKey += "top:" + GetPixelString(imageModel.Top) + ";left:" + GetPixelString(imageModel.Left) + ";";
                }
                else if (imageModel.PageInfo != null)
                {
                    styleKey += "top:" + GetPixelString(imageModel.PageInfo.ActualTop) + ";left:" + GetPixelString(imageModel.PageInfo.ActualLeft) + ";";
                }

                cssClasses = GetCssClass(styleKey);
                bool hasLink = false;

                if (imageModel.ImageProperties.Border != null)
                {
                    cssClasses += " " + this.GetBorderStyles(imageModel.ImageProperties.Border);
                }
                if (imageModel.ReportItem != null && imageModel.ReportItem.ActionInfo != null)
                {
                    var action = imageModel.ReportItem.ActionInfo.Actions.First();
                    writer.WriteStartElement("a");
                    this.GetHyperLink(action, writer);
                    hasLink = true;
                }

                writer.WriteStartElement("DIV");
                writer.WriteAttributeString("class", cssClasses);
                writer.WriteStartElement("IMG");
                cssClasses = "";

                if (imageModel.ImageProperties.Padding != null)
                {
                    cssClasses += " " + GetPaddingCss(imageModel.ImageProperties.Padding);
                }

                writer.WriteAttributeString("class", cssClasses);
                writer.WriteAttributeString("height", height + "px");
                writer.WriteAttributeString("width", width + "px");

                string str3 = "data:image/" + imageModel.ImageFormat.ToString().ToLower();
                string imgstream = Convert.ToBase64String(stream.ToArray());
                writer.WriteAttributeString("src", str3 + ";base64," + imgstream);
                writer.WriteAttributeString("alt", imageModel.Name);
                writer.WriteEndElement();
                writer.WriteEndElement();
                if (hasLink)
                {
                    writer.WriteEndElement();
                }
                writer.WriteRaw("\n");
            }
            catch
            {

            }
        }

#if ! SILVERLIGHT
#if !SyncfusionFramework3_5
        private void ProcessMapModel(IReportItemModeler reportModel, XmlTextWriter writer)
        {
            MapModel mapModel = (MapModel)reportModel;
            System.Drawing.Image mapImage = System.Drawing.Image.FromStream(mapModel.GetImageStream());
            int width = Convert.ToInt32(mapModel.Width);
            int height = Convert.ToInt32(mapModel.Height);
            Bitmap image = new Bitmap(mapImage, width, height);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            styleKey = "position:absolute;width:" + width + "px;height:" + height + "px;";

            if (mapModel.PageInfo != null)
            {
                styleKey += "top:" + GetPixelString(mapModel.PageInfo.ActualTop) + ";left:" + GetPixelString(mapModel.PageInfo.ActualLeft) + ";";
            }
            else
            {
                styleKey += "top:" + GetPixelString(mapModel.Top) + ";left:" + GetPixelString(mapModel.Left) + ";";
            }

            cssClasses = GetCssClass(styleKey);

            if (mapModel.MapProperties.Border != null && mapModel.MapProperties.Border.Default != null)
            {
                cssClasses += " " + GetBorderStyles(mapModel.MapProperties.Border);
            }

            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", cssClasses);
            writer.WriteStartElement("IMG");
            string str3 = "data:image/jpeg";
            string imgstream = Convert.ToBase64String(stream.ToArray());
            writer.WriteAttributeString("src", str3 + ";base64," + imgstream);
            writer.WriteAttributeString("alt", mapModel.Name);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.WriteRaw("\n");
        }
#endif
#endif

#if SILVERLIGHT
        private void ProcessRectangleModel(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessRectangleModel(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            RectangleModel rectangleModel = reportModel as RectangleModel;
            styleKey = "position:absolute;width:" + GetPixelString(rectangleModel.Width) + ";height:" + GetPixelString(rectangleModel.Height) + ";";

            if (rectangleModel.IsTablixChild && rectangleModel.IsTablixInnerChild)
            {
                styleKey += "top:" + GetPixelString(rectangleModel.Top) + ";left:" + GetPixelString(rectangleModel.Left) + ";";
            }
            else if (!rectangleModel.IsTablixChild)
            {
                double top = (this.m_CurrentPage == 0 ? rectangleModel.PageInfo.ActualTop : 0);
                double left = rectangleModel.PageInfo.ActualLeft;
                styleKey += "top:" + GetPixelString(top) + ";left:" + GetPixelString(left) + ";";
            }

            cssClasses = GetCssClass(styleKey);
            cssClasses += " " + GetCssClass("background-color:" + rectangleModel.RectItemExpPro.BackgroundColor + ";");

            if (rectangleModel.RectItemExpPro.Border != null)
            {
                cssClasses += " " + this.GetBorderStyles(rectangleModel.RectItemExpPro.Border);
            }

            if (rectangleModel.ReportItem.Style != null && rectangleModel.ReportItem.Style.BackgroundImage != null)
            {
                cssClasses += " " + GetCssClass(GetBackgroundImage(rectangleModel.ReportItem.Style.BackgroundImage.Value));
            }

            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", cssClasses);

            if (rectangleModel.IsTablixChild && rectangleModel.IsTablixInnerChild)
            {
                foreach (var model in rectangleModel.ReportItemModelers)
                {
                    ProcessReportModel(model, writer);
                }
            }
            writer.WriteRaw("");
            writer.WriteEndElement();
        }

#if SILVERLIGHT
        private void ProcessTablixModel(IReportItemModeler reportModel, XmlWriter writer)
#else
        private void ProcessTablixModel(IReportItemModeler reportModel, XmlTextWriter writer)
#endif
        {
            TablixModel tablixModel = (TablixModel)reportModel;
            int pageNo = reportModel.PageInfo.BelongsTo[this.m_CurrentPage];
            int top = (int)(pageNo == 0 ? tablixModel.PageInfo.ActualTop : 0);
            styleKey = "position:absolute;left:" + GetPixelString(tablixModel.PageInfo.ActualLeft) + ";top:" + top + "px;";
            styleKey += "width:" + GetPixelString(tablixModel.PageInfo.ActualWidth) + ";height:" + GetPixelString(tablixModel.PageInfo.ActualHeight) + ";";

            writer.WriteStartElement("DIV");
            writer.WriteAttributeString("class", GetCssClass(styleKey));
            var pageInfo = tablixModel.PageSizes[pageNo];
            int columnCount = pageInfo.ColumnIndices.Count();
            int rowCount = pageInfo.RowIndices.Count();

            if (columnCount > 0)
            {
                writer.WriteStartElement("TABLE");
                writer.WriteAttributeString("cellspacing", "0");
                writer.WriteAttributeString("cellpadding", "0");

                if (tablixModel.ReportItem != null && tablixModel.ReportItem.Style != null)
                {
                    cssClasses = GetCssClass("margin:0pt;text-align:left;border-collapse:collapse;");
                    cssClasses += " " + this.GetBorderStyle(tablixModel.ReportItem.Style);
                    writer.WriteAttributeString("class", cssClasses);
                }

                writer.WriteStartElement("tbody");
                writer.WriteStartElement("tr");

                for (int column = 0; column < columnCount; column++)
                {
                    writer.WriteStartElement("td");
                    writer.WriteAttributeString("class", GetCssClass("width:" + this.GetPixelString(tablixModel.ColumnWights[column]) + ";"));
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                int rowIndex = 0;

                for (int row = 0; row < rowCount; row++)
                {
                    rowIndex = pageInfo.RowIndices[row] - 1;
                    writer.WriteStartElement("tr");
                    cssClasses = GetCssClass("padding:0pt;margin:0pt;text-align:left;height:" + GetPixelString(tablixModel.RowHeights[rowIndex]) + ";");
                    writer.WriteAttributeString("class", cssClasses);

                    for (int column = 0; column < columnCount; column++)
                    {
                        var cellInfo = tablixModel.Data[rowIndex][column];
                        int rowSpan = 1;
                        int colSpan = 1;

                        var coveredCells = from range in tablixModel.CoveredRanges
                                           where range.Top == rowIndex && range.Left == column
                                           select range;

                        if (coveredCells.Count() > 0)
                        {
                            var coveredCell = coveredCells.First();
                            rowSpan = (coveredCell.Bottom - coveredCell.Top) + 1;
                            colSpan = (coveredCell.Right - coveredCell.Left) + 1;
                        }
                        if (rowSpan < 1 && cellInfo != null)
                        {
                            rowSpan = cellInfo.RowSpan;
                        }
                        if (colSpan < 1 && cellInfo != null)
                        {
                            colSpan = cellInfo.ColumnSpan;
                        }
                        if (cellInfo != null)
                        {
                            if (this.ReportModel.EnableVirtualEvaluation)
                            {
                                cellInfo.CurrentKey = new List<int>();
                                cellInfo.CurrentKey.Add(rowIndex);
                                cellInfo.CurrentKey.Add(column);
                                cellInfo.Evaluate();
                            }

                            writer.WriteStartElement("td");
                            cssClasses = "";
                            bool hasLink = false;

                            if (cellInfo.ItemModel.ReportItem != null && cellInfo.ItemModel.ReportItem.ActionInfo != null)
                            {
                                var action = cellInfo.ItemModel.ReportItem.ActionInfo.Actions.First();
                                writer.WriteStartElement("a");
                                this.GetHyperLink(action, writer);
                                hasLink = true;
                            }
                            if (rowSpan > 1)
                            {
                                writer.WriteAttributeString("rowspan", rowSpan.ToString());
                            }
                            if (colSpan > 1)
                            {
                                writer.WriteAttributeString("colspan", colSpan.ToString());
                            }
                            if (cellInfo.Border != null)
                            {
                                cssClasses = this.GetBorderStyles(cellInfo.Border);
                            }

                            StringBuilder content = new StringBuilder();
                            switch (cellInfo.ItemModel.ModelType)
                            {
                                case ModelType.TextBoxModel:
                                    var textbox = (TextboxModel)cellInfo.ItemModel;
                                    styleKey = "";

                                    if (textbox.TextBoxProperties.BackGroundColor != null)
                                    {
                                        cssClasses += " " + GetCssClass("background-color:" + textbox.TextBoxProperties.BackGroundColor + ";");
                                    }
                                    foreach (var paragraph in textbox.ParaExpval)
                                    {
                                        foreach (var run in paragraph.Runs)
                                        {
                                            cssClasses += " " + GetTextRunStyle(run);
                                            styleKey += "text-align:" + paragraph.TextAlignment + ";" + "vertical-align:" + textbox.TextBoxProperties.VerticalAlignment.ToString() + ";";
                                            content.Append(run.Text);
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(styleKey))
                                    {
                                        cssClasses += " " + GetCssClass(styleKey);
                                    }
                                    if (textbox.TextBoxProperties.Padding != null)
                                    {
                                        cssClasses += " " + GetPaddingCss(textbox.TextBoxProperties.Padding);
                                    }

                                    writer.WriteAttributeString("class", cssClasses);
                                    writer.WriteRaw(content.ToString());
                                    content = null;

                                    break;

                                case ModelType.ImageModel:
                                    var imagemodel = (ImageModel)cellInfo.ItemModel;

                                    if (imagemodel.ImageProperties.Padding != null)
                                    {
                                        cssClasses += " " + GetPaddingCss(imagemodel.ImageProperties.Padding);
                                    }

                                    writer.WriteAttributeString("class", cssClasses);
                                    object imgData = imagemodel.ImageData;
                                    byte[] array = (byte[])imgData;
                                    writer.WriteStartElement("IMG");
                                    writer.WriteAttributeString("height", this.GetPixelString(imagemodel.Height));
                                    writer.WriteAttributeString("width", this.GetPixelString(imagemodel.Width));
                                    string str3 = "data:image/png";
                                    string imgstream = Convert.ToBase64String(array);
                                    writer.WriteAttributeString("src", str3 + ";base64," + imgstream);
                                    writer.WriteAttributeString("alt", imagemodel.Name);
                                    writer.WriteEndElement();
                                    break;

                                case ModelType.RectangleModel:
                                    writer.WriteAttributeString("class", GetCssClass("position:relative;display:block;"));
                                    var rect = cellInfo.ItemModel as RectangleModel;
                                    var temp = this.m_CurrentPage;
                                    var page = 0;
                                    if (rect.PageInfo != null)
                                    {
                                        page = rect.PageInfo.BelongsTo[this.m_CurrentPage];
                                    }
                                    this.m_CurrentPage = page;
                                    foreach (var model in cellInfo.ItemModel.ReportItemModelers)
                                    {
                                        ProcessReportModel(model, writer);
                                    }
                                    this.m_CurrentPage = temp;
                                    break;
#if ! SILVERLIGHT
                                case ModelType.GaugeModel:
                                    var gaugemodel = (GaugeModel)cellInfo.ItemModel;
                                    System.Drawing.Image gaugeimage = System.Drawing.Image.FromStream(gaugemodel.GetImageStream());
                                    int gaugewidth = Convert.ToInt32(gaugemodel.Width);
                                    int gaugeheight = Convert.ToInt32(gaugemodel.Height);
                                    Bitmap gbitimage = new Bitmap(gaugeimage, gaugewidth, gaugeheight);
                                    MemoryStream gaugestream = new MemoryStream();
                                    gbitimage.Save(gaugestream, ImageFormat.Jpeg);
                                    writer.WriteAttributeString("class", cssClasses);
                                    writer.WriteStartElement("IMG");
                                    writer.WriteAttributeString("height", this.GetPixelString(gaugeheight));
                                    writer.WriteAttributeString("width", this.GetPixelString(gaugewidth));
                                    string gaugeimgstream = Convert.ToBase64String(gaugestream.ToArray());
                                    writer.WriteAttributeString("src", "data:image/png;base64," + gaugeimgstream);
                                    writer.WriteAttributeString("alt", gaugemodel.Name);
                                    writer.WriteEndElement();
                                    break;
                                case ModelType.ChartModel:
                                    var chartmodel = (ChartModel)cellInfo.ItemModel;
                                    System.Drawing.Image chartimage = System.Drawing.Image.FromStream(chartmodel.GetImageStream());
                                    int chartwidth = Convert.ToInt32(chartmodel.Width);
                                    int chartheight = Convert.ToInt32(chartmodel.Height);
                                    Bitmap cbitimage = new Bitmap(chartimage, chartwidth, chartheight);
                                    MemoryStream chartstream = new MemoryStream();
                                    cbitimage.Save(chartstream, ImageFormat.Jpeg);
                                    writer.WriteAttributeString("class", cssClasses);
                                    writer.WriteStartElement("IMG");
                                    writer.WriteAttributeString("height", this.GetPixelString(chartheight));
                                    writer.WriteAttributeString("width", this.GetPixelString(chartwidth));
                                    string chartimgstream = Convert.ToBase64String(chartstream.ToArray());
                                    writer.WriteAttributeString("src", "data:image/png;base64," + chartimgstream);
                                    writer.WriteAttributeString("alt", chartmodel.Name);
                                    writer.WriteEndElement();
                                    break;
                                case ModelType.MapModel:
                                    var mapmodel = (MapModel)cellInfo.ItemModel;
                                    System.Drawing.Image mapimage = System.Drawing.Image.FromStream(mapmodel.GetImageStream());
                                    int mapwidth = Convert.ToInt32(mapmodel.Width);
                                    int mapheight = Convert.ToInt32(mapmodel.Height);
                                    Bitmap mbitimage = new Bitmap(mapimage, mapwidth, mapheight);
                                    MemoryStream mapstream = new MemoryStream();
                                    mbitimage.Save(mapstream, ImageFormat.Jpeg);
                                    writer.WriteAttributeString("class", cssClasses);
                                    writer.WriteStartElement("IMG");
                                    writer.WriteAttributeString("height", this.GetPixelString(mapheight));
                                    writer.WriteAttributeString("width", this.GetPixelString(mapwidth));
                                    string mapimgstream = Convert.ToBase64String(mapstream.ToArray());
                                    writer.WriteAttributeString("src", "data:image/png;base64," + mapimgstream);
                                    writer.WriteAttributeString("alt", mapmodel.Name);
                                    writer.WriteEndElement();
                                    break;
#endif
                               case ModelType.TablixModel:
                                    writer.WriteAttributeString("class", GetCssClass("position:relative;display:block;"));
                                    var tmodel = cellInfo.ItemModel as TablixModel;
                                    var temp1 = this.m_CurrentPage;
                                    var page1 = 0;
                                    if (tmodel.PageInfo != null)
                                    {
                                        page1 = tmodel.PageInfo.BelongsTo[this.m_CurrentPage];
                                    }
                                    this.m_CurrentPage = page1;
                                     ProcessReportModel(tmodel, writer);
                                    this.m_CurrentPage = temp1;

                                    break;
                            }
                            if (hasLink)
                            {
                                writer.WriteEndElement();
                            }

                            writer.WriteEndElement();
                            cellInfo.DisposeEvalObjects();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private string ProcessRunText(string run)
        {
            if (run.Contains("Globals.TotalPages"))
            {
                run = run.Replace("Globals.TotalPages", this.m_totalPages.ToString());
            }
            else if (run.Contains("Globals.ExecutionTime"))
            {
                run = run.Replace("Globals.ExecutionTime", System.DateTime.Now.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture));
            }
            else if (run.Contains("User.Language"))
            {
                run = run.Replace("User.Language", System.Globalization.CultureInfo.CurrentCulture.Name.ToString());
            }

            return run;
        }

#if SILVERLIGHT
        private void GetHyperLink(Syncfusion.RDL.DOM.Action action, XmlWriter writer)
#else
        private void GetHyperLink(Syncfusion.RDL.DOM.Action action, XmlTextWriter writer)
#endif
        {
            if (action.BookmarkLink != null)
            {
                writer.WriteAttributeString("class", GetCssClass("text-decoration:none;color:black;"));
                writer.WriteAttributeString("href", action.BookmarkLink);
            }
            else if (action.Drillthrough != null)
            {
                writer.WriteAttributeString("class", GetCssClass("text-decoration:none;color:black;"));
                writer.WriteAttributeString("href", action.Drillthrough.ReportName);
            }
            else if (action.Hyperlink != null)
            {
                writer.WriteAttributeString("class", GetCssClass("text-decoration:none;color:black;"));
                writer.WriteAttributeString("href", action.Hyperlink);
            }
        }

        private string GetTextRunStyle(TextRunExpval run)
        {
            if (run != null)
            {
                styleKey = "font-family:" + run.Style.Font.FontFamily + ";font-size:" + GetPixelString(run.Style.Font.FontSize);
                styleKey += ";font-weight:" + run.Style.Font.FontWeight + ";font-style:" + run.Style.Font.FontStyle + ";color:" + run.Style.TextColor + ";";

                if (!styleValues.ContainsKey(styleKey))
                {
                    this.styleValues.Add(styleKey, "sf" + m_styleCount);
                    m_styleCount++;
                }

                return this.styleValues[styleKey];
            }

            return string.Empty;
        }

        private string GetBorderStyles(BorderExpval border)
        {
            styleKey = "";

            if (border.Default != null)
            {
                if (border.Default.BorderStyle != RDL.DOM.BorderStyles.None && border.Default.Thickness == 0)
                {
                    styleKey += "border:1px ";
                }
                else
                {
                    styleKey += "border:" + border.Default.Thickness + "px ";
                }

                styleKey += border.Default.BorderStyle + " " + border.Default.BorderBrush + ";";
            }
            if (border.LeftBorder != null)
            {
                styleKey += "border-left:" + border.LeftBorder.Thickness + "px " + border.LeftBorder.BorderStyle + " " + border.LeftBorder.BorderBrush + ";";
            }
            if (border.RightBorder != null)
            {
                styleKey += "border-right:" + border.RightBorder.Thickness + "px " + border.RightBorder.BorderStyle + " " + border.RightBorder.BorderBrush + ";";
            }
            if (border.TopBorder != null)
            {
                styleKey += "border-top:" + border.TopBorder.Thickness + "px " + border.TopBorder.BorderStyle + " " + border.TopBorder.BorderBrush + ";";
            }
            if (border.BottomBorder != null)
            {
                styleKey += "border-bottom:" + border.BottomBorder.Thickness + "px " + border.BottomBorder.BorderStyle + " " + border.BottomBorder.BorderBrush + ";";
            }

            if (!string.IsNullOrEmpty(styleKey))
            {
                return GetCssClass(styleKey);
            }

            return string.Empty;
        }

        private string GetBorderStyle(RDL.DOM.Style style)
        {
            styleKey = "";
            string width = "1pt";

            if (style.Border != null)
            {
                if (style.Border.Style != null)
                {
                    width = style.Border.Width == null ? width : style.Border.Width.size;
                }
                styleKey += "border:" + width + " " + style.Border.Style + " " + style.Border.Color + "; ";
            }
            if (style.LeftBorder != null)
            {
                if (style.LeftBorder.Style != null)
                {
                    width = style.LeftBorder.Width == null ? width : style.LeftBorder.Width.size;
                }
                styleKey += "border-left:" + width + " " + style.LeftBorder.Style + " " + style.LeftBorder.Color + "; ";
            }
            if (style.RightBorder != null)
            {
                if (style.RightBorder.Style != null)
                {
                    width = style.RightBorder.Width == null ? width : style.RightBorder.Width.size;
                }
                styleKey += "border-right:" + width + " " + style.RightBorder.Style + " " + style.RightBorder.Color + "; ";
            }
            if (style.TopBorder != null)
            {
                if (style.TopBorder.Style != null)
                {
                    width = style.TopBorder.Width == null ? width : style.TopBorder.Width.size;
                }
                styleKey += "border-top:" + width + " " + style.TopBorder.Style + " " + style.TopBorder.Color + "; ";
            }
            if (style.BottomBorder != null)
            {
                if (style.BottomBorder.Style != null)
                {
                    width = style.BottomBorder.Width == null ? width : style.BottomBorder.Width.size;
                }
                styleKey += "border-bottom:" + width + " " + style.BottomBorder.Style + " " + style.BottomBorder.Color + "; ";
            }

            if (!string.IsNullOrEmpty(styleKey))
            {
                return GetCssClass(styleKey);
            }

            return string.Empty;
        }

        private string GetCssClass(string m_styleKey)
        {
            if (!styleValues.ContainsKey(m_styleKey))
            {
                this.styleValues.Add(m_styleKey, "sb" + m_styleCount);
                m_styleCount++;
            }

            return this.styleValues[m_styleKey];
        }

        private string GetPaddingCss(ThicknessExpval padding)
        {
            styleKey = "padding:" + padding.Top + "px " + padding.Right + "px " + padding.Bottom + "px " + padding.Left + "px;";

            if (!styleValues.ContainsKey(styleKey))
            {
                this.styleValues.Add(styleKey, "sp" + m_styleCount);
                m_styleCount++;
            }

            return this.styleValues[styleKey];
        }

        private void WriteHtml()
        {
            this.WriteHead();
            this.WriteBody();
            this.m_writer.WriteEndElement();
        }

        private void WriteHead()
        {
            this.m_writer.WriteDocType("HTML", "-//W3C//DTD HTML 4.01 Transitional//EN", null, null);
            this.m_writer.WriteStartElement("HTML");
            this.m_writer.WriteStartElement("HEAD");
            this.m_writer.WriteRaw("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            this.m_writer.WriteRaw("<meta http-equiv=\"Content-Style-Type\" content=\"text/css\" />");
            this.m_writer.WriteRaw("\n");
            this.m_writer.WriteStartElement("style");
            this.m_writer.WriteAttributeString("type", "text/css");
            this.m_writer.WriteRaw("\n");
            this.WriteCss();
            this.m_writer.WriteEndElement();
            this.m_writer.WriteRaw("\n");
            this.m_writer.WriteEndElement();
        }

        private void WriteCss()
        {
            foreach (var style in this.styleValues)
            {
                if (!style.Key.Equals("header") && !style.Key.Equals("footer"))
                {
                    styleKey = "." + style.Value + "{" + style.Key + "}\n";
                    this.m_writer.WriteRaw(styleKey);
                }
            }
        }

        private void WriteBody()
        {
            this.m_writer.WriteStartElement("body");
            this.m_writer.WriteAttributeString("style", "BORDER: 0px; MARGIN: 0px; PADDING: 0px;");
            this.m_writer.WriteRaw("\n");

            foreach (var content in this.bodyContents)
            {
                this.m_writer.WriteStartElement("DIV");
                this.m_writer.WriteAttributeString("ID", "page" + content.Key + 1);

                if (this.m_Headerstream != null)
                {
                    this.m_writer.WriteRaw("\n");
                    this.m_writer.WriteStartElement("DIV");
                    if (styleValues.ContainsKey("header"))
                        this.m_writer.WriteAttributeString("class", styleValues["header"]);
                    string temp = this.ConvertToString(this.m_Headerstream);
                    if (temp.Contains("Globals.PageNumber"))
                        temp = temp.Replace("Globals.PageNumber", (content.Key + 1).ToString());
                    this.m_writer.WriteRaw(temp);
                    this.m_writer.WriteEndElement();
                }

                this.m_writer.WriteRaw("\n");
                this.m_writer.WriteStartElement("DIV");
                this.m_writer.WriteRaw(this.ConvertToString(content.Value));
                this.m_writer.WriteEndElement();

                if (this.m_FooterStream != null)
                {
                    this.m_writer.WriteRaw("\n");
                    this.m_writer.WriteStartElement("DIV");
                    if (styleValues.ContainsKey("footer"))
                        this.m_writer.WriteAttributeString("class", styleValues["footer"]);
                    string temp = this.ConvertToString(this.m_FooterStream);
                    if (temp.Contains("Globals.PageNumber"))
                        temp = temp.Replace("Globals.PageNumber", (content.Key + 1).ToString());
                    this.m_writer.WriteRaw(temp);
                    this.m_writer.WriteEndElement();
                }

                this.m_writer.WriteRaw("<br/>");
                this.m_writer.WriteRaw("<hr/>");
                this.m_writer.WriteEndElement();
                this.m_writer.WriteRaw("\n");
            }

            this.m_writer.WriteRaw("\n");
            this.m_writer.WriteEndElement();
        }

        private string GetPixelString(double value)
        {
            return Math.Round(value, 2) + "px";
        }

        private string ConvertToString(MemoryStream memoryStream)
        {
            memoryStream.Position = 0;
            StreamReader reader = new StreamReader(memoryStream);
            string temp = reader.ReadToEnd();

            return temp;
        }

        private string GetBackgroundImage(string imageName)
        {
            Base64ImageConverter base64ImageConverter = new Base64ImageConverter();
            foreach (Syncfusion.RDL.DOM.EmbeddedImage embeddedImage in this.ReportModel.Report.EmbeddedImages)
            {
                if (embeddedImage.Name == imageName)
                {
                    if (!imageValues.ContainsKey(imageName))
                    {
                        BitmapImage bitMapImage = new BitmapImage();
                        bitMapImage = (BitmapImage)base64ImageConverter.ConvertToImage(embeddedImage.ImageData);
#if ! SILVERLIGHT
                        Bitmap image = new Bitmap(Image.FromStream(bitMapImage.StreamSource));
                        MemoryStream stream = new MemoryStream();
                        image.Save(stream, ImageFormat.Png);

#elif WINRT 
                        byte[] data = Encoding.UTF8.GetBytes(embeddedImage.ImageData);
                        MemoryStream stream = new MemoryStream(data);
#else
                        MemoryStream stream = new MemoryStream(GetBytes(bitMapImage));

#endif
                        imageValues.Add(imageName, stream);
                    }

                    string tag = Convert.ToBase64String(imageValues[imageName].ToArray());
#if SILVERLIGHT

                    tag = "background-image:url(data:image/ " + embeddedImage.MIMEType.Substring(6) + ";base64," + tag + ");";
#else
                    tag = "background-image:url(data:image/png;base64," + tag + ");";
#endif

                    return tag;
                }
            }
            return null;
        }

#if SILVERLIGHT && !WINRT 
        public byte[] GetBytes(BitmapImage bi)
        {
            WriteableBitmap wbm = new WriteableBitmap(bi);
            return ToByteArray(wbm);
        }


        public static byte[] ToByteArray(WriteableBitmap bmp)
        {
            //Init buffer
            int w = bmp.PixelWidth;
            int h = bmp.PixelHeight;
            int[] p = bmp.Pixels;
            int len = p.Length;
            byte[] result = new byte[4 * w * h];

            // Copy pixels to buffer
            for (int i = 0, j = 0; i < len; i++, j += 4)
            {
                int color = p[i];
                result[j + 0] = (byte)(color >> 24); // A
                result[j + 1] = (byte)(color >> 16); // R
                result[j + 2] = (byte)(color >> 8);  // G
                result[j + 3] = (byte)(color);       // B
            }

            return result;
        }
#endif

        private BitmapImage BufferImage(object imageData)
        {
            return (BitmapImage)this.Buffer(imageData, typeof(BitmapImage), null, System.Globalization.CultureInfo.InvariantCulture);
        }

        private BitmapImage Buffer(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            byte[] imageData = null;

            string s = value as string;

            if (s != null)
            {
                imageData = System.Convert.FromBase64String(s);
            }
            if (value is byte[])
            {
                imageData = value as byte[];
            }

            if (imageData != null)
            {
                BitmapImage bi = new BitmapImage();
                MemoryStream stream = new MemoryStream();
#if !SILVERLIGHT
                int offset = 78;
                stream.Write(imageData, offset, imageData.Length - offset);
                stream.Seek(0, SeekOrigin.Begin);
                bi.BeginInit();
                bi.StreamSource = stream;
                bi.EndInit();
#elif WINRT
                bi.SetSource(new MemoryStream(imageData) as Windows.Storage.Streams.IRandomAccessStream);
#else
                bi.SetSource(new MemoryStream(imageData));
#endif
                return bi;
            }
            return null;
        }

        private ImageFormat GetImageFormat(string format)
        {
            if (format == ImageFormat.Emf.ToString())
            {
                return ImageFormat.Emf;
            }
            if (format == ImageFormat.Wmf.ToString())
            {
                return ImageFormat.Wmf;
            }

            return ImageFormat.Jpeg;
        }

        #endregion
    }
}
