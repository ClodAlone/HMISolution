using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Bornander.UI.Commands.Tolerant;
using System.Windows;
using System.Net.Sockets;
using System.Net;

namespace Bornander.UI.Commands.Test
{
    public enum Warnings
    {
        OverwriteFile,
        NoFileSuffix
    }

    public class ViewModel : INotifyPropertyChanged
    {
        private string filename = @"C:\Temp\SomeFile.txt";

        public ICommand SaveFileCommand { get; private set; }

        private readonly IDialogDisplayer displayer = new AcknowledgableDialogDisplayer();
        private readonly IWarningRepository<Warnings> repository = new EnumRepository<Warnings>();

        public ViewModel()
        {
            SaveFileCommand = new TolerantCommand<Warnings>(displayer, repository, x => true, SaveFile);
        }

        private void SaveFile(object parameter, IEnumerable<Warnings> ignorableWarnings)
        {
            if (File.Exists(Filename) && !TolerantCommand<Warnings>.IsWarningIgnored(Warnings.OverwriteFile, ignorableWarnings))
                throw new CommandWarningException(String.Format("This will overwrite the file \"{0}\", are you sure you want to do that?", Filename), Warnings.OverwriteFile);

            if (!Filename.Contains('.') && !TolerantCommand<Warnings>.IsWarningIgnored(Warnings.NoFileSuffix, ignorableWarnings))
                throw new CommandWarningException(String.Format("The filename \"{0}\" has no file suffix, are you sure you want keep it like that?", Filename), Warnings.NoFileSuffix);

            File.WriteAllText(Filename, "Go do that voodoo that you do so well.");
        }

        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                Notify("Filename");
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void Notify(params string[] propertyNames)
        {
            if (PropertyChanged != null)
            {
                foreach (string propertyName in propertyNames)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public object Warning { get; set; }
    }
}
