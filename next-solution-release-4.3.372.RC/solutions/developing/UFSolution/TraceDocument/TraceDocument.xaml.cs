using System.Windows;
using System.Windows.Documents;
using System.Diagnostics;
using System;
using System.Windows.Threading;
using Utilities;

namespace TraceTextBox
{
    public partial class TraceDocument : FlowDocument, ITraceTextSink
    {
        private TraceListener _listener;
        private Paragraph _current;

        int maxLines = 1000;
        public int MaxLines
        {
            get
            {
                return maxLines;
            }
            set
            {
                if (maxLines == value)
                    return;
                maxLines = value;
            }
        }

        bool listen;
        public bool Listen
        {
            get
            {
                return listen;
            }
            set
            {
                if (listen == value)
                    return;
                listen = value;
                if (listen && _listener == null)
                {
                    _listener = new TraceTextSource(this);
                    Trace.Listeners.Add(_listener);
                }
                else if (!listen && _listener != null)
                {
                    Trace.Listeners.Remove(_listener);
                    _listener.Dispose();
                    _listener = null;
                }
            }
        }

        public TraceDocument()
        {
            AutoAttach = false;
            InitializeComponent();
        }

        public bool AutoAttach { get; set; }

        public void Event(string msg, TraceEventType eventType)
        {
            Dispatcher.BeginInvokeIfRequired(
            (Action)delegate
            {
                if (_current == null)
                    AddParagraph(msg, eventType.ToString());
                else
                    Append(msg, eventType.ToString());
            });
        }

        public void Fail(string msg)
        {
            AddParagraph(msg, "Fail");
        }

        private void Append(string msg, string style)
        {
            Dispatcher.BeginInvokeIfRequired(
            (Action)delegate
            {
                _current.Inlines.Add(new Run(msg.TrimEnd('\n')));
                if (_current.Inlines.Count > MaxLines)
                    _current.Inlines.Remove(_current.Inlines.FirstInline);
                _current = msg.EndsWith("\n") ? null : _current;
            });
        }

        private void AddParagraph(string msg, string style)
        {
            Dispatcher.BeginInvokeIfRequired(
            (Action)delegate
            {
                Paragraph p = new Paragraph(new Run(msg.TrimEnd('\n')));
                p.Style = (Style)(Resources[style]);
                if (p.Style == null)
                    p.Style = (Style)(Resources["Information"]);

                Blocks.Add(p);
                _current = msg.EndsWith("\n") ? null : p;
                if (Blocks.Count > MaxLines)
                    Blocks.Remove(Blocks.FirstBlock);
            });
        }

        private void Document_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoAttach && _listener == null)
            {
                _listener = new TraceTextSource(this);
                Trace.Listeners.Add(_listener);
            }
        }

        private void Document_Unloaded(object sender, RoutedEventArgs e)
        {
            if (AutoAttach && _listener != null)
            {
                Trace.Listeners.Remove(_listener);
                _listener.Dispose();
                _listener = null;
            }
        }
    }
}
