using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Utilities.WPF;
using DevExpress.Xpf.WindowsUI;
using DevExpress.Xpf.Grid;
using WPFUtilities;
using DevExpress.Xpf.Grid.TreeList;
using ScreenSettings;
using System.Runtime.Serialization;
using System.Text;
using ViewModelLib;
using System.ComponentModel;
using System.Windows.Threading;
using WPFUtilities.PropertyDataTemplate;
using System.IO.IsolatedStorage;
using log4net;

namespace ScreenManager
{
    /// <summary>
    /// Interaction logic for NewScreenType.xaml
    /// </summary>
    public partial class NewScreen : UserControl, IDataErrorInfo
    {

        #region ScreenName
        public static readonly DependencyProperty ScreenNameProperty = DependencyProperty.Register("ScreenName", typeof(string), typeof(NewScreen), new UIPropertyMetadata(string.Empty));
        public string ScreenName
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(ScreenNameProperty);
            }
            set
            {
                SetValue(ScreenNameProperty, value);
            }
        }
        #endregion
        #region ScreenBackBrush
        public static readonly DependencyProperty ScreenBackBrushProperty = DependencyProperty.Register("ScreenBackBrush", typeof(Brush), typeof(NewScreen), new UIPropertyMetadata(Brushes.DarkGray));
        public Brush ScreenBackBrush
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (Brush)GetValue(ScreenBackBrushProperty);
            }
            set
            {
                SetValue(ScreenBackBrushProperty, value);
            }
        }
        #endregion
        #region ScreenHeight
        public static readonly DependencyProperty ScreenHeightProperty = DependencyProperty.Register("ScreenHeight", typeof(double), typeof(NewScreen), new UIPropertyMetadata(Properties.Settings.Default.DefaultScreenHeight));
        public double ScreenHeight
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ScreenHeightProperty);
            }
            set
            {
                SetValue(ScreenHeightProperty, value);
            }
        }
        #endregion
        #region ScreenWidth
        public static readonly DependencyProperty ScreenWidthProperty = DependencyProperty.Register("ScreenWidth", typeof(double), typeof(NewScreen), new UIPropertyMetadata(Properties.Settings.Default.DefaultScreenWidth));
        public double ScreenWidth
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (double)GetValue(ScreenWidthProperty);
            }
            set
            {
                SetValue(ScreenWidthProperty, value);
            }
        }
        #endregion

        public String fileName { get; set; }
        public String xamlCode { get; set; }
        public String xamlSettingCode { get; set; }
        public String sourcefileName { get; set; }

        bool isPopup;
        bool isDataContextChanging;
        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);

        RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(
                        param => CallResetCommand(param),
                        param => IsEnableResetCommand
                        );
                }
                return _resetCommand;
            }
        }

        public bool IsEnableResetCommand
        { get
            {
                return true;
            }
        }

        private void CallResetCommand(Object param)
        {
            switch (param)
            {
                case "0":
                    ScreenWidth = defScreenWidth;
                    break;
                case "1":
                    ScreenHeight = defScreenHeight;
                    break;
                case "2":
                    ScreenBackBrush = defScreenBackBrush;
                    break;
                case "3":
                    ScreenName = fileName;
                    break;
                default:
                    break;
            }
        }
        double defScreenWidth;
        double defScreenHeight;
        Brush defScreenBackBrush;
        string storeFileName;
        bool bLoaded;
        public NewScreen(string storeFileName)
        {
            InitializeComponent();
            DataContext = this;
            Loaded += (o, e) =>
            {
                if(!bLoaded)
                {
                    bLoaded = false;
                    this.storeFileName = storeFileName;
                    ScreenWidth = defScreenWidth = Properties.Settings.Default.DefaultScreenWidth;
                    ScreenHeight = defScreenHeight = Properties.Settings.Default.DefaultScreenHeight;
                    ScreenBackBrush = defScreenBackBrush = TryFindResource("Nuance30") as Brush;
                    ScreenName = fileName;
                    LoadData();
                    var wnd = this.FindParent<Window>();
                    wnd.Closing += (s, c) =>
                    {
                        if (!IsValid)
                            return;

                        isDataContextChanging = true;
                        SaveToFileDefaultXaml();
                        if(wnd.DialogResult == true)
                            SaveData();
                        DataContext = xamlCode;
                        isDataContextChanging = false;
                    };
                }
            };
        }

        private void SaveToFileDefaultXaml()
        {
            Canvas cnvs = new Canvas();
            double width = ScreenWidth;
            double height = ScreenHeight;
            cnvs.Width = width;
            cnvs.Height = height;
            cnvs.Background = ScreenBackBrush;

            xamlCode = XamlWriter.Save(cnvs);
            xamlSettingCode = GetXAMLSettings(width, height);
        }
        private string GetXAMLSettings(double width, double height)
        {
            var doc = new ScreenDocument();

            doc.Width = width;
            doc.Height = height;

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            String _result = string.Empty;
            using (MemoryStream output = new MemoryStream())
            {
                using (XmlWriter writer = XmlDictionaryWriter.Create(output, settings))
                {
                    try
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(ScreenDocument));
                        serializer.WriteObject(writer, doc);
                    }
                    finally
                    {
                        writer.Close();
                    }
                }
                _result = Encoding.UTF8.GetString(output.ToArray());
            }

            return string.Format("{0}", _result);
        }
        #region Save Load Recents Settings

        public class SaveDataStorage
        {
            public double width;
            public double height;
            public string backcolor;
        }

        void SaveData()
        {
            using (new WaitCursor())
            {
                try
                {
                    if (File.Exists(storeFileName))
                        File.Delete(storeFileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(storeFileName));

                    using (var fileStream = File.Open(storeFileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = System.Text.Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                        {
                            try
                            {
                                var data = new SaveDataStorage()
                                {
                                    width = ScreenWidth,
                                    height = ScreenHeight,
                                    backcolor = XamlWriter.Save(ScreenBackBrush)
                                };

                                DataContractSerializer serializer = new DataContractSerializer(typeof(SaveDataStorage));
                                serializer.WriteObject(writer, data);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                                LogError(Properties.Resources.FailedToSaveScreenSettings, ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError(Properties.Resources.FailedToSaveScreenSettings, ex);
                }
            }
        }

        void LogError(String errMsg, Exception ex)
        {
            logGeneral.Error(errMsg, ex);
            System.Diagnostics.Trace.TraceError(ex.ToString());
        }

        void LoadData()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            using (new WaitCursor())
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(storeFileName));

                    using (var fileStream = File.Open(storeFileName, FileMode.Open))
                    {
                            XmlReaderSettings settings = new XmlReaderSettings
                            {
                                ConformanceLevel = ConformanceLevel.Document,
                                CloseInput = true
                            };

                            using (XmlReader reader = XmlReader.Create(fileStream, settings))
                            {
                                try
                                {
                                    DataContractSerializer serializer = new DataContractSerializer(typeof(SaveDataStorage));
                                    var data = serializer.ReadObject(reader) as SaveDataStorage;

                                    ScreenWidth = data.width;
                                    ScreenHeight = data.height;
                                    ScreenBackBrush = XamlReader.Parse(data.backcolor) as Brush;
                                }
                                catch (Exception ex)
                                {
                                    reader.Close();
                                }
                            }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        #endregion


        #region IDataErrorInfo Members

        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }

        string PerformValidation(string propertyName)
        {
            if (propertyName == "ScreenWidth")
            {
                if (ScreenWidth <= 0 || ScreenWidth > ScreenSettings.Properties.Settings.Default.ScreenMaxWidth)
                    return Properties.Resources.ScreenSizeError;
            }
            if (propertyName == "ScreenHeight")
            {
                if (ScreenHeight <= 0 || ScreenHeight > ScreenSettings.Properties.Settings.Default.ScreenMaxHeight)
                    return Properties.Resources.ScreenSizeError;
            }
            return null;
        }

        bool IsValid
        {
            get
            {
                return PerformValidation("ScreenWidth") == null && PerformValidation("ScreenHeight") == null;
            }
        }

        #endregion
    }
}
