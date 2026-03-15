using DevExpress.Xpf.Core;
using MSZUtilsServiceHelper;
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

namespace MSZUtils.Controls
{
    public enum MessageType
    {
        Error,
        Warning,
        About,
        Info
    }
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class MessageContent : UserControl
    {
        public MessageContent(String message, MessageType type)
        {
            InitializeComponent();
            content.Text = message;

            switch (type)
            {
                case MessageType.Error:
                    error.Visibility = Visibility.Visible;
                    error.Width = 64;
                    error.Height = 64;
                    error.Stretch = Stretch.Uniform;
                    content.FontSize = 22;
                    content.FontWeight = FontWeights.Bold;
                    content.Foreground = Brushes.Red;
                    break;
                case MessageType.Warning:
                    warning.Visibility = Visibility.Visible;
                    warning.Width = 64;
                    warning.Height = 64;
                    warning.Stretch = Stretch.Uniform;
                    content.FontSize = 22;
                    content.FontWeight = FontWeights.Bold;
                    break;
                case MessageType.About:
                    request.Visibility = Visibility.Visible;
                    break;
                case MessageType.Info:
                    info.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
