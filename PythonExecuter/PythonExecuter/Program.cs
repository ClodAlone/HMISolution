using System;
using Python.Runtime;

class Program
{
    static void Main(string[] args)
    {
        Runtime.PythonDLL = "C:\\Users\\cfior\\AppData\\Local\\Programs\\Python\\Python311\\python311.dll";

        // Inizializza l'interprete di Python
        PythonEngine.Initialize();

        // Avvia il debug di Python
        using (Py.GIL()) // Acquisisce il Global Interpreter Lock (GIL) di Python
        {
            dynamic sys = Py.Import("sys");
            dynamic pdb = Py.Import("pdb");

            // Imposta il file Python da debuggare
            string pythonFile = @"c:\temp\file.py";
            dynamic debugger = pdb.Pdb();
            debugger.run("exec(open('" + pythonFile + "').read())");
        }

        // Termina l'interprete di Python
        PythonEngine.Shutdown();
    }
}
