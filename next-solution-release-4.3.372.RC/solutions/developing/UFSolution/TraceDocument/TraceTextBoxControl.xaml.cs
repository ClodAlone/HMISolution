using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace TraceTextBox
{
	/// <summary>
	/// Interaction logic for TraceTextBox.xaml
	/// </summary>
	public partial class TraceTextBoxControl : UserControl, ITraceTextSink
	{
		private TraceListener _listener;

		public TraceTextBoxControl()
		{
            AutoAttach = true;
			InitializeComponent();
		}

        public bool AutoAttach { get; set; }

		public void Event(string msg, TraceEventType eventType)
		{
            textBox1.AppendText(msg);
            textBox1.ScrollToEnd();
		}

		public void Fail(string msg)
		{
            textBox1.AppendText(msg);
            textBox1.ScrollToEnd();
		}		

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoAttach && _listener == null)
            {
                _listener = new TraceTextSource(this);
                Trace.Listeners.Add(_listener);
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_listener != null)
            {
                Trace.Listeners.Remove(_listener);
                _listener.Dispose();
                _listener = null;
            }
        }
	}
}
