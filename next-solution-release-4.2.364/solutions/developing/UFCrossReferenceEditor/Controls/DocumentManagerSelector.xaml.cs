using DocumentManager.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Runtime.Serialization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using UFCrossReferenceEditor.Document;
using System.Reflection;

namespace UFCrossReferenceEditor.Controls
{
    /// <summary>
    /// Interaction logic for DocumentManagerSelector.xaml
    /// </summary>
    public partial class DocumentManagerSelector : UserControl
    {
        public List<Selection> SlectionList { get; protected set; }
        public List<Selection> CrossReferenceTypeList { get; protected set; }
        public DocumentManagerSelector(List<IDocumentManager> listDocumentManagers)
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                var document = DataContext as CREditorDocument;
                if (document == null)
                    return;
                SlectionList = new List<Selection>();
                CrossReferenceTypeList = new List<Selection>();
                listDocumentManagers.ForEach(d => SlectionList.Add(new Selection() { DocumentManager = d, Name = d.TypeTitle }));
                List<CrossReferenceType> typeList = Enum.GetValues(typeof(CrossReferenceType)).Cast<CrossReferenceType>().ToList();
                typeList.ForEach(d => CrossReferenceTypeList.Add(new Selection() { Name = d.ToString(), ReferenceType = d}));
                LoadLayout(document.Title);
                listView.ItemsSource = SlectionList;
                listView1.ItemsSource = CrossReferenceTypeList;
            };

            Unloaded += (s, e) =>
            {
                var document = DataContext as CREditorDocument;
                if (document == null)
                    return;

                SaveLayout(document.Title);
            };
        }
        public class Selection
        {
            public Boolean IsEnabled { get; set; }
            public IDocumentManager DocumentManager { get; set; }
            public CrossReferenceType ReferenceType { get; set; }
            public String Name { get; set; }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            SlectionList.ForEach(s => s.IsEnabled = true);
            listView.ItemsSource = null;
            listView.ItemsSource = SlectionList;
        }

        private void SelectNone_Click(object sender, RoutedEventArgs e)
        {
            SlectionList.ForEach(s => s.IsEnabled = false);
            listView.ItemsSource = null;
            listView.ItemsSource = SlectionList;
        }




        #region Isolated Storage

        static String GetStoreFileName(String title)
        {
            return String.Format("{0}.{1}.CRDocumentManagerSelector.dat", title, System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
        }

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

        void SaveLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.Create, isoStorage))
                {
                    XmlWriterSettings settings = new XmlWriterSettings
                    {
                        Indent = true,
                        OmitXmlDeclaration = false,
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = new SettingsStorage();
                        settingsStorage.ManagerList = (from s in SlectionList
                                                       where s.IsEnabled select s.DocumentManager.TypeLabel).ToList();
                        settingsStorage.CrossReferenceTypeList = (from s in CrossReferenceTypeList
                                                                  where s.IsEnabled
                                                                  select s.ReferenceType.ToString()).ToList();
                        serializer.WriteObject(writer, settingsStorage);
                    }
                }
            }
            catch
            { }
        }

        void LoadLayout(String title)
        {
            try
            {
                var isoStorage = GetStorage();
                if (null == isoStorage || string.IsNullOrEmpty(title))
                    return;

                using (var stream = new IsolatedStorageFileStream(GetStoreFileName(title), FileMode.OpenOrCreate, isoStorage))
                {
                    XmlReaderSettings settings = new XmlReaderSettings
                    {
                        ConformanceLevel = ConformanceLevel.Document,
                        CloseInput = true
                    };

                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        var serializer = new DataContractSerializer(typeof(SettingsStorage));
                        var settingsStorage = serializer.ReadObject(reader) as SettingsStorage;
                        SlectionList.ForEach(s => s.IsEnabled = settingsStorage.ManagerList.Contains(s.DocumentManager.TypeLabel));
                        CrossReferenceTypeList.ForEach(s => s.IsEnabled = settingsStorage.CrossReferenceTypeList.Contains(s.ReferenceType.ToString()));
                    }
                }
            }
            catch
            { }
        }

        #endregion
    }


    [DataContract(Name = "SettingsStorage")]
    class SettingsStorage
    {
        #region Members
        [DataMember]
        public List<String> ManagerList;
        [DataMember]
        public List<String> CrossReferenceTypeList;
        #endregion
    }
}
