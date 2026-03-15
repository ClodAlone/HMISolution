using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ADPluginInterfaces;
using Utilities;

namespace ADFirebase.UI
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

                    string testo = txtText.Text;
                    string txtFCMTokenPath = this.txtFCMTokenPath.Text;
                    if (pluginWpfEditing.Init(conn))
                    {
                        int result = pluginWpfEditing.OnSendMessage(new ADPluginBase.Message() { FCMTokenPath = txtFCMTokenPath, Textmessage = testo });
                        string le = pluginWpfEditing.GetLastError();
                        if (result != (int)ADFirebaseError.ADFirebaseErrorNoError)
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
