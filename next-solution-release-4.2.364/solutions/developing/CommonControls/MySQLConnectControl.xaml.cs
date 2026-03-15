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

namespace CommonControls
{
    /// <summary>
    /// Interaction logic for MySQLConnectControl.xaml
    /// </summary>
    public partial class MySQLConnectControl : UserControl
    {
        public MySQLConnectControl()
        {
            InitializeComponent();
        }

        public string GetDataBaseConnectionString()
        {
            string connectionString = GetServerConnectionString();
            return String.Format("{0};initial catalog={1}", connectionString, tbDatabase.Text);
        }

        string GetServerConnectionString()
        {
            string connectionString = String.Format("data source={0};user id={1};password={2}", tbServer.Text, tbLogin.Text, tbPassword.Password);
            return connectionString;
        }
    }
}
