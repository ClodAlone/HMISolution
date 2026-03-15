using System;
using System.Collections.Generic;
using System.IO;
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
using ICSharpCode.AvalonEdit.Highlighting;
using Python.Runtime;


namespace WpfPythonExecuter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Imposta lo schema di highlighting per il linguaggio Python
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Python");

            // Aggiungi un gestore di eventi per il clic del pulsante "Avvia Debug"
            debugButton.Click += DebugButton_Click;
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            Runtime.PythonDLL = "C:\\Users\\cfior\\AppData\\Local\\Programs\\Python\\Python311\\python311.dll";

            // Ottieni il codice Python dal contenuto dell'editor di testo
            string pythonCode = textEditor.Text;

            // Inizializza l'interprete di Python
            PythonEngine.Initialize();

            // Create a custom stdin stream for the debugger to use
            var stdinStream = new MemoryStream();
            var stdinWriter = new StreamWriter(stdinStream);
            stdinWriter.AutoFlush = true;
            System.Console.SetIn(new StreamReader(stdinStream));

            // Run the Python code with the debugger
            using (Py.GIL()) // Acquires the Global Interpreter Lock (GIL) of Python
            {
                dynamic pdb = Py.Import("pdb");

                // Set the Python code to debug
                dynamic debugger = pdb.Pdb();
                debugger.stdin = System.Console.In;

                using (var scope = Py.CreateScope())
                {
                    dynamic globals = scope;
                    dynamic locals = scope;

                    PythonEngine.Exec(pythonCode, globals, locals);
                    debugger.run("exec(compile('''" + pythonCode + "''', '<stdin>', 'exec'))");
                }
            }

            // Termina l'interprete di Python
            PythonEngine.Shutdown();
        }

    }
}

/*

namespace MonacoIntegrationExample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Imposta lo schema di highlighting per il linguaggio Python
            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("Python");

            // Aggiungi un gestore di eventi per il clic del pulsante "Avvia Debug"
            debugButton.Click += DebugButton_Click;
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            // Ottieni il codice Python dal contenuto dell'editor di testo
            string pythonCode = textEditor.Text;

            // Inizializza l'interprete di Python
            PythonEngine.Initialize();

            // Esegui il codice Python con il debugger
            using (Py.GIL()) // Acquisisce il Global Interpreter Lock (GIL) di Python
            {
                dynamic pdb = Py.Import("pdb");

                // Imposta il codice Python da debuggare
                dynamic debugger = pdb.Pdb();
                debugger.run(pythonCode);
            }

            // Termina l'interprete di Python
            PythonEngine.Shutdown();
        }
    }
}

*/