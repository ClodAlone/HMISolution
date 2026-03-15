#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.Windows.Markup;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Grid.GridUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public static class GridUtility
    {

        /// <summary>
        /// Converts the Paragraph value as Xaml string.
        /// </summary>
        /// <param name="paragraph"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string ConvertParagraphToXaml(Paragraph paragraph)
        {
            StringBuilder Xamlstring = new StringBuilder();
            System.Windows.Media.SolidColorBrush ParagraphForeground = paragraph.Foreground as System.Windows.Media.SolidColorBrush;
            Xamlstring.Append("<Paragraph FontSize=\"" + paragraph.FontSize + "\" FontFamily=\"" + paragraph.FontFamily + "\" Foreground=\"" + Color.FromArgb(ParagraphForeground.Color.A, ParagraphForeground.Color.R, ParagraphForeground.Color.G, ParagraphForeground.Color.B) + "\" FontWeight=\"" + paragraph.FontWeight + "\" FontStyle=\"" + paragraph.FontStyle + "\" FontStretch=\"" + paragraph.FontStretch + "\" TextAlignment=\"" + paragraph.TextAlignment + "\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");
            for (int i = 0; i < paragraph.Inlines.Count; i++)
            {
                var content = paragraph.Inlines[i];
                if (content is Run)
                {
                    Run inline = paragraph.Inlines[i] as Run;
                    System.Windows.Media.SolidColorBrush runforeground = inline.Foreground as System.Windows.Media.SolidColorBrush;
                    string Textdecoration = "";
                    if (inline.TextDecorations != null)
                    {
                        Textdecoration = "TextDecorations=\"Underline\"";
                    }
                    else
                    {
                        Textdecoration = "";
                    }
                    Xamlstring.Append("<Run FontSize=\"" + inline.FontSize + "\" FlowDirection=\"" + inline.FlowDirection + "\" FontFamily=\"" + inline.FontFamily + "\" Foreground=\"" + Color.FromArgb(runforeground.Color.A, runforeground.Color.R, runforeground.Color.G, runforeground.Color.B) + "\" FontWeight=\"" + inline.FontWeight + "\" Text=\"" + inline.Text + "\" " + Textdecoration + " FontStyle=\"" + inline.FontStyle + "\" FontStretch=\"" + inline.FontStretch + "\"/>");
                }

                else if (content is Hyperlink)
                {
                    Hyperlink Link = content as Hyperlink;
                    System.Windows.Media.SolidColorBrush linkforeground = Link.Foreground as System.Windows.Media.SolidColorBrush;
                    System.Windows.Media.SolidColorBrush MouseOverforeground = Link.MouseOverForeground as System.Windows.Media.SolidColorBrush;
                    string Textdecoration = "";
                    if (Link.TextDecorations != null)
                    {
                        Textdecoration = " TextDecorations=\"Underline\"";
                    }
                    else
                    {
                        Textdecoration = "";
                    }
                    Xamlstring.Append("<Hyperlink FontSize=\"" + Link.FontSize + "\"  Foreground=\"" + Color.FromArgb(linkforeground.Color.A, linkforeground.Color.R, linkforeground.Color.G, linkforeground.Color.B) + "\" FontFamily=\"" + Link.FontFamily + "\"" + Textdecoration + " FontWeight=\"" + Link.FontWeight + "\"  NavigateUri=\"" + Link.NavigateUri + "\" MouseOverForeground=\"#FFED6E00\" FontStyle=\"" + Link.FontStyle + "\" FontStretch=\"" + Link.FontStretch + "\">");
                    if (Link.Inlines[0] is Run)
                    {
                        Run inline = Link.Inlines[0] as Run;
                        System.Windows.Media.SolidColorBrush runforeground = inline.Foreground as System.Windows.Media.SolidColorBrush;
                        string decoration = "";
                        if (inline.TextDecorations != null)
                        {
                            decoration = " TextDecorations=\"Underline\"";
                        }
                        else
                        {
                            decoration = "";
                        }
                        Xamlstring.Append("<Run FontSize=\"" + inline.FontSize + "\" FlowDirection=\"" + inline.FlowDirection + "\" FontFamily=\"" + inline.FontFamily + "\" Foreground=\"" + Color.FromArgb(runforeground.Color.A, runforeground.Color.R, runforeground.Color.G, runforeground.Color.B) + "\" FontWeight=\"" + inline.FontWeight + "\" Text=\"" + inline.Text + "\"" + decoration + " FontStyle=\"" + inline.FontStyle + "\" FontStretch=\"" + inline.FontStretch + "\"/>");

                    }

                    Xamlstring.Append("</Hyperlink>");
                }
                else if (content is InlineUIContainer)
                    {
                    Xamlstring.Append("<InlineUIContainer>");
                    InlineUIContainer uicontainer = content as InlineUIContainer;
                    if (uicontainer.Child is Image)
                        {

                        Xamlstring.Append("<Image Height=\"" + (uicontainer.Child as Image).Height + "\"  Margin=\"" + (uicontainer.Child as Image).Margin.ToString() + "\"  Stretch=\"" + (uicontainer.Child as Image).Stretch + "\" Width=\"" + (uicontainer.Child as Image).Width + "\" Source=\"" + ((uicontainer.Child as Image).Source as System.Windows.Media.Imaging.BitmapImage).UriSource.OriginalString + "\" UseLayoutRounding=\"" + (uicontainer.Child as Image).UseLayoutRounding + "\" />");
                        }
                    Xamlstring.Append("</InlineUIContainer>");
                    }
                else if (content is LineBreak)
                {
                    Xamlstring.Append("<LineBreak />");
                }
            }
            Xamlstring.Append("</Paragraph>");
            return Xamlstring.ToString();
        }

        /// <summary>
        /// Converts the Xaml String as Paragraph value. 
        /// </summary>
        /// <param name="xamlstring"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static Paragraph ConvertXamlToParagraph(string xamlstring)
        {
            return XamlReader.Load(xamlstring) as Paragraph;

        }
       
        


    }
}
