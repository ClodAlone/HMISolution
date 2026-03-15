using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using ADPluginInterfaces;
using Utilities;

namespace ADTelegram.UI
{
    /// <summary>
    /// Interaction logic for PluginTestEditor.xaml
    /// </summary>
    public partial class PluginTestEditor : UserControl
    {
        string conn = null;
        ADModel.ADPlugin plugin = null;
        bool alreadyLoaded = false;
        public PluginTestEditor()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (alreadyLoaded)
                    return;

                alreadyLoaded = true;

                List<object> lista = DataContext as List<object>;
                if (lista == null || lista.Count < 2)
                    return;

                conn = lista[0] as string;
                plugin = lista[1] as ADModel.ADPlugin;
#if DEBUG
 //               txtNumber.Text = "123456789";
                txtText.Text = "messaggio di prova";
#endif

            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var Cursor = new WaitCursor())
            {
                if (plugin != null)
                {
                    var uidll = string.Format("{0}{1}", ADServerInfo.ADServerInfo.GetServerFolder(), plugin.AssemblyName);

                    IPlugin pluginWpfEditing = null;
                    try
                    {
                        var types = Assembly.LoadFile(uidll).GetTypes();
                        var list = (from t in types.AsParallel()
                                    where !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t)
                                    select (IPlugin)Activator.CreateInstance(t)).ToList();

                        pluginWpfEditing = list[0];
                    }
                    catch (Exception ex)
                    {
                        Cursor.Release();
                        MessageBox.Show(String.Format(Properties.Resources.PluginNotFound, uidll));
                    }

                    if (pluginWpfEditing == null)
                        return;
                    //string address = txtNumber.Text;
                    string testo = txtText.Text;
                    string ChatID = txtChatID.Text;
                    if (pluginWpfEditing.Init(conn))
                    {
                        int result = pluginWpfEditing.OnSendMessage(new ADPluginBase.Message() { ChatID = ChatID, Textmessage = testo });
                        string le = pluginWpfEditing.GetLastError();
                        if (result != (int)ADTelegramError.ADTelegramErrorNoError)
                        {
                            if (le != null)
                            {
                                Cursor.Release();
                                MessageBox.Show(le, Properties.Resources.TestCaption);
                            }
                            else
                            {
                                Cursor.Release();
                                MessageBox.Show(Properties.Resources.TestError, Properties.Resources.TestCaption);
                            }
                        }
                        else
                        {
                            Cursor.Release();
                            MessageBox.Show(Properties.Resources.TestMessageSent, Properties.Resources.TestCaption);
                        }
                    }
                }
            }
        }
    }
}
