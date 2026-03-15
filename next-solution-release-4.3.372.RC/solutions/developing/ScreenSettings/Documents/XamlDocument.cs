using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Media;
using System.Xml;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Controls;
using Utilities.WPF;
#endif
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI;
#endif

namespace ScreenSettings
{
    public class XamlDocument : INotifyPropertyChanged
    {
#region Constructors

        public XamlDocument(string folder)
        {
            _Folder = folder;
        }

#endregion

#region Fields

        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;

#endregion

#region Public Properties

        private string _Folder = "";
        public string Folder
        {
            get 
            {
                return _Folder; 
            }
            set
            {
                if (_Folder != value)
                {
                    _Folder = value;
                    NotifyPropertyChanged("Folder");
                    NotifyPropertyChanged("FullPath");
                }
            }
        }

        private string _Filename;
        public string Filename
        {
            get 
            {
                if (String.IsNullOrEmpty(_Filename))
                {
                    return TemporaryFilename;
                }
                else
                {
                    return _Filename;
                }
            }
            set 
            {
                if (_Filename != value)
                {
                    _Filename = value;
                    NotifyPropertyChanged("Filename");
                    NotifyPropertyChanged("FullPath");
                }
            }
        }

        string _TemporaryFilename = "";
        public string TemporaryFilename
        {
            get 
            {
                if (string.IsNullOrEmpty(_TemporaryFilename))
                {
                    string temp = "";
                    /*
                    if (TempFilenameCount == 0)
                    {
                        temp = TempFilenamePreface + ".xaml";
                    }
                    else
                    {
                        temp = TempFilenamePreface + TempFilenameCount + ".xaml";
                    }
                    */
                    _TemporaryFilename = temp;
                    TempFilenameCount++;
                }
                return _TemporaryFilename; 
            }
        }

#if !NET_STANDARD
        private Canvas _LoadedCanvas;
        public Canvas LoadedCanvas
        {
            get { return _LoadedCanvas; }
            set
            {
                if (_LoadedCanvas != value)
                {
                    _LoadedCanvas = value;
                    NotifyPropertyChanged("LoadedCanvas");
                }
            }
        }
#endif
        private string _SourceText;
        public string SourceText
        {
            get { return _SourceText; }
            set
            {
                if (_SourceText != value)
                {
                    _SourceText = value;
                    NotifyPropertyChanged("SourceText");
                }
            }
        }

        public bool UsingTemporaryFilename
        {
            get 
            {
                return (String.IsNullOrEmpty(_Filename));
            }
        }

        public string FullPath
        {
            get
            {
                if (String.IsNullOrEmpty(Filename))
                {
                    return Path.Combine(Folder, TemporaryFilename);
                }
                else
                {
                    return Path.Combine(Folder, Filename);
                }
            }
            set
            {
                Folder = Path.GetDirectoryName(value);
                Filename = Path.GetFileName(value);
            }
        }

        public string BackupPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(FullPath), Path.GetFileNameWithoutExtension(FullPath) + ".backup");
            }
        }

        private ImageSource _PreviewImage;
        public ImageSource PreviewImage
        {
            get
            {
                if (_PreviewImage == null)
                {
                    // look for a preview image on disk and load it
                    // Path.Combine(Path.GetDirectoryName(FullPath), Path.GetFileNameWithoutExtension(FullPath) + ".preview");
                }

                return _PreviewImage;
            }
            set
            {
                if (_PreviewImage != value)
                {
                    _PreviewImage = value;
                    NotifyPropertyChanged("PreviewImage");
                }
            }
        }

        #endregion

        #region Protected, Internal and Private Methods

#if !NET_STANDARD
        public void InitializeCanvas(Canvas code)
        {
            _LoadedCanvas = code;
        }
#endif
        public void InitializeSourceText(string text)
        {
            _SourceText = text;
        }

        private bool SaveFile(string fullPath)
        {
            File.WriteAllText(fullPath, SourceText);
            return true;
        }

#if !WINDOWS_UWP
        internal void AddProtectionCode(Guid guid)
        {
            /*
            var cv = ParseLoadedDocument();
            if (cv != null)
            { 
                cv.Uid = String.Format("{0}", guid);
                _SourceText = cv.XamlWriterFormatted();
            }
            */
            if (String.IsNullOrEmpty(_SourceText))
                return;
            var tagToFind = "<Canvas ";
            int found = _SourceText.IndexOf(tagToFind);
            if (found < 0)
                return;
            var s = String.Format("Uid=\"{0}\" ", guid);
            if (!_SourceText.Contains(s))
                _SourceText = _SourceText.Insert(found + tagToFind.Length, String.Format(" {0}", s));
        }

        internal void RemoveProtectionCode(Guid guid)
        {
            /*
            var cv = ParseLoadedDocument();
            if (cv != null)
            {
                cv.Uid = null;
                _SourceText = cv.XamlWriterFormatted();
            }
            */
            if (String.IsNullOrEmpty(_SourceText))
                return;
            var s = String.Format("Uid=\"{0}\"", guid);
            int found = _SourceText.IndexOf(s);
            if (found < 0)
                return;
            _SourceText = _SourceText.Replace(s, "");
        }
#endif

#endregion

#region Public Methods

        public bool SaveAs(string fullPath)
        {
            if (SaveFile(fullPath))
            {
                this.FullPath = fullPath;
                return true;
            }
            return false;
        }

        public bool Save()
        {
            if (SaveFile(FullPath))
            {
                return true;
            }
            return false;
        }

        public bool SaveBackup()
        {
            return SaveFile(BackupPath);
        }

#endregion

#region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion

#if !NET_STANDARD
        public Canvas ParseLoadedDocument()
        {
            if (LoadedCanvas != null)
                return LoadedCanvas;
            if (SourceText != null)
            {
#if !WINDOWS_UWP
                MemoryStream ms = null;
                StreamWriter sw = null;
// handle the in place preparsing (this actually updates the source in the editor)
                SourceText = PreParse(SourceText);
                string str = SourceText;

                // handle the in memory preparsing (this happens behind the scenes all in memory)
                // str = DeSilverlight(str);
#endif
                try
                {
#if !WINDOWS_UWP
                    ms = new MemoryStream(str.Length); 
                    sw = new StreamWriter(ms);
                    sw.Write(str);
                    sw.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    Uri baseUri = null;
                    try
                    {
                        baseUri = new Uri(String.Format("{0}/", Folder));
                    }
                    catch (Exception ex)
                    {                        
                    }

                    ParserContext pc = new ParserContext
                    {
                        // System.IO.Packaging.PackUriHelper.Create()
                        BaseUri = baseUri
                    };

                    object content = XamlReader.Load(ms, pc);
#else
                    object content = XamlReader.Load(SourceText);
#endif
                    var canvas = new Canvas();

                    if (content is Canvas)
                    {
                        return content as Canvas;
                    }
#if !WINDOWS_UWP
                    else if (content is InkCanvas)
                    {
                        var inkcanvas = content as InkCanvas;
                        while (inkcanvas.Children.Count > 0)
                        {
                            UIElement child = inkcanvas.Children[0] as UIElement;

                            double left = InkCanvas.GetLeft(child);
                            double top = InkCanvas.GetTop(child);

                            left = double.IsNaN(left) ? 0 : left;
                            top = double.IsNaN(top) ? 0 : top;

                            Canvas.SetLeft(child, left);
                            Canvas.SetTop(child, top);

                            inkcanvas.Children.Remove(child);
                            canvas.Children.Add(child);
                        }

                        canvas.Height = inkcanvas.Height;
                        canvas.Width = inkcanvas.Width;
                        canvas.Background = inkcanvas.Background;

                        return canvas;
                    }
#endif
                    else if (content is Window)
                    {
                        Window wnd = content as Window;
                        UIElement uie = wnd.Content as UIElement;
                        if (uie != null)
                        {
                            wnd.Content = null;
                            wnd.Close();

                            canvas.Children.Add(uie);
                            return canvas;
                        }
                    }
                    else if (content is Page)
                    {
                        Page wnd = content as Page;
                        UIElement uie = wnd.Content as UIElement;
                        if (uie != null)
                        {
                            wnd.Content = null;
                            canvas.Children.Add(uie);
                            return canvas;
                        }
                    }

                    canvas.Children.Add(content as UIElement);
                    return canvas;
                }
                finally
                {
#if !WINDOWS_UWP
                    if (sw != null)
                    {
                        sw.Close();
                    }
#endif
                }
            }

            return null;
        }
#endif
        //private static string DeSilverlight(string str)
        //{
        //    if (Properties.Settings.Default.EnablePseudoSilverlight)
        //    {
        //        str = str.Replace("http://schemas.microsoft.com/client/2007", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
        //    }

//    return str;
//}

#if !WINDOWS_UWP && !NET_STANDARD
        private string PreParse(string str)
        {
            while (str.Contains("?COLOR"))
            {
                str = ReplaceOnce(str, "?COLOR", GetRandomColor().ToString());
            }

            while (str.Contains("?NAMEDCOLOR"))
            {
                str = ReplaceOnce(str, "?NAMEDCOLOR", GetRandomColorName().ToString());
            }

            return str;
        }
        private static string ReplaceOnce(string str, string oldValue, string newValue)
        {
            int index = str.IndexOf(oldValue);
            string s = str;

            s = s.Remove(index, oldValue.Length);
            s = s.Insert(index, newValue);

            return s;
        }

        Random R = new Random();

        private Color GetRandomColor()
        {
            return Color.FromRgb((byte)R.Next(0, 255), (byte)R.Next(0, 255), (byte)R.Next(0, 255));
        }
        private string GetRandomColorName()
        {
            string[] colors = new string[] 
            { "AliceBlue", "Aquamarine", "Azure", "Bisque", "BlanchedAlmond", "Burlywood", 
                "CadetBlue", "Chartreuse", "Chocolate", "Coral", "CornflowerBlue", "Cornsilk", 
                "DodgerBlue", "FloralWhite", "Gainsboro", "Ghostwhite", "Honeydew", "HotPink", 
                "IndianRed", "LightSalmon", 
                "Mintcream", "MistyRose", "Moccasin", "NavajoWhite", "Oldlace", "PapayaWhip", 
                "PeachPuff", "Peru", "SaddleBrown", "Seashell", "Thistle", "Tomato", "WhiteSmoke" 
            };
            return colors[R.Next(0, colors.Length - 1)];
        }
#endif
        //private void ReportError(Exception e)
        //{
        //    if (e is XamlParseException)
        //    {
        //        XamlParseException x = (XamlParseException)e;
        //        ErrorLineNumber = x.LineNumber;
        //        ErrorLinePosition = x.LinePosition;
        //    }
        //    else
        //    {
        //        ErrorLineNumber = 0;
        //        ErrorLinePosition = 0;
        //    }

        //    Exception inner = e;

        //    while (inner.InnerException != null) inner = inner.InnerException;

        //    ErrorText = inner.Message;
        //    ErrorText = ErrorText.Replace("\r", "");
        //    ErrorText = ErrorText.Replace("\n", "");
        //    ErrorText = ErrorText.Replace("\t", "");

        //    // get rid of everything after "Line" if it is in the last 30 characters 
        //    int pos = ErrorText.LastIndexOf("Line");
        //    if (pos > 0 && pos > (ErrorText.Length - 50))
        //    {
        //        ErrorText = ErrorText.Substring(0, pos);
        //    }
        //}

    }
}
