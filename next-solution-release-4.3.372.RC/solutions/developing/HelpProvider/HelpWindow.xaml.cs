using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace HelpProvider
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {

        static Dictionary<String, Rect> mapPositions = new Dictionary<String, Rect>();
        static Dictionary<String, WindowState> mapStates = new Dictionary<String, WindowState>();

        static bool bLoaded;
        static bool bExitEventHandlerRegisterd;
        public HelpWindow(FrameworkElement dialogContent)
        {
            InitializeComponent();

            WindowStyle = System.Windows.WindowStyle.ToolWindow;

            HelpContent.Content = dialogContent;
            var reference = String.Format("{0}-{1}", dialogContent.GetType(), dialogContent.Name);

            if (!bExitEventHandlerRegisterd)
            {
                bExitEventHandlerRegisterd = true;
                //Application.Current.Dispatcher.InvokeIfRequired(() =>
                //{
                Application.Current.Exit += (o, e) =>
                {
                    try
                    {
                        SaveDialogPositions();
                    }
                    catch (Exception ex)
                    {
                        //logGeneral.Error(WPFUtilities.Properties.Resources.FailedToSaveWindowsPositions, ex);
                    }
                };
                //});
            }

                Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        try
                        {
                            LoadDialogPositions();
                        }
                        catch (Exception ex)
                        {
                            //logGeneral.Error(WPFUtilities.Properties.Resources.FailedToLoadWindowsPositions, ex);
                        }
                    }

                    if (mapPositions.ContainsKey(reference))
                    {
                        SizeToContent = SizeToContent.Manual;
                        WindowStartupLocation = WindowStartupLocation.Manual;

                        try
                        {
                            // var rect = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point((int)Top, (int)Left));
                            if (mapPositions[reference].Left + mapPositions[reference].Width > SystemParameters.VirtualScreenWidth ||
                                mapPositions[reference].Top + mapPositions[reference].Height > SystemParameters.VirtualScreenHeight ||
                                mapPositions[reference].Left < SystemParameters.VirtualScreenLeft ||
                                mapPositions[reference].Top < SystemParameters.VirtualScreenTop)
                                WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            else
                            {
                                Top = mapPositions[reference].Top;
                                Left = mapPositions[reference].Left;
                                Width = mapPositions[reference].Width;
                                Height = mapPositions[reference].Height;
                            }
                            //if (Left + Width > rect.Right)
                            //    Left -= (Left + Width - rect.Right);
                            //if (Top + Height > rect.Bottom)
                            //    Top -= (Top + Height - rect.Bottom);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    if (mapStates.ContainsKey(reference))
                        WindowState = mapStates[reference];
                };
                Closed += (o, e) =>
                {
                    var rect = new Rect(Left, Top, Width, Height);
                    mapPositions.Remove(reference);
                    mapPositions.Add(reference, rect);
                    mapStates.Remove(reference);
                    mapStates.Add(reference, WindowState);

                    if (Application.Current.Dispatcher.CheckAccess() && Application.Current.MainWindow != null)
                    {
                        //Application.Current.MainWindow.Dispatcher.InvokeIfRequired(() =>
                        //{
                            Application.Current.MainWindow.Activate();
                        //});
                    }

                    ClearContent();
                };

        }
            public void ClearContent()
        {
            // DialogContent.Content = null;
        }
            #region Save Load Recents

        const String StoreFileName = "SavedDialogs";

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        static void SaveDialogPositions()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
                return;

            using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    ConformanceLevel = System.Xml.ConformanceLevel.Auto,
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    try
                    {
                        DataContractSerializer serializer1 = new DataContractSerializer(mapPositions.GetType());
                        serializer1.WriteObject(writer, mapPositions);
                        DataContractSerializer serializer2 = new DataContractSerializer(mapStates.GetType());
                        serializer2.WriteObject(writer, mapStates);
                    }
                    catch (Exception ex)
                    {
                        writer.Close();
                    }
                }
            }
        }

        static void LoadDialogPositions()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                isoStorage.GetFileNames(StoreFileName).Length <= 0)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseInput = true
                };

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    try
                    {
                        var serializer1 = new DataContractSerializer(mapPositions.GetType());
                        mapPositions = serializer1.ReadObject(reader) as Dictionary<String, Rect>;

                        var serializer2 = new DataContractSerializer(mapStates.GetType());
                        mapStates = serializer2.ReadObject(reader) as Dictionary<String, WindowState>;
                    }
                    catch (Exception ex)
                    {
                        mapStates.Clear();
                        mapPositions.Clear();
                        reader.Close();
                    }
                }
            }
        }

        #endregion
        
    }
}
