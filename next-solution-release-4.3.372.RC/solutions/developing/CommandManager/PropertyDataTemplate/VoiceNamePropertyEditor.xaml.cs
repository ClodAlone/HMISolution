using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Speech.Synthesis;

namespace CommandManager.PropertyDataTemplate
{
    /// <summary>
    /// Interaction logic for VoiceNamePropertyEditor.xaml
    /// </summary>
    public partial class VoiceNamePropertyEditor : UserControl
    {
        public VoiceNamePropertyEditor()
        {
            InitializeComponent();
        }

        private void DlgButton_Click_Clear(object sender, RoutedEventArgs e)
        {
            uri.Tag = null;
        }
        bool bInit;
        private void uriLabel_PopupOpening(object sender, EventArgs e)
        {
            try
            {
                if (!bInit)
                {
                    if (uriLabel.Items.Count == 0)
                    {
                        using (SpeechSynthesizer synth = new SpeechSynthesizer())
                        {
                            var listvoices = synth.GetInstalledVoices()?.Select(x => x.VoiceInfo.Name).ToList();
                            //error, no voices!!!
                            if (listvoices.Count > 0)
                                uriLabel.ItemsSource = listvoices;
                        }

                        bInit = true;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
