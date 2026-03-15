using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace S7TCP.UI
{
    public class ImportTagViewModel : INotifyPropertyChanged
    {

        public ImportTagViewModel()
        {
            _selectedTags = new List<IImportTag>();
            SelectedItemChanged = new Action<IImportTag>(SelectedItemChangedHandler);
        }

        public Action<IImportTag> SelectedItemChanged { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private List<IImportTag> _selectedTags;

        private IEnumerable<IImportTag> _importtags;
        public IEnumerable<IImportTag> ImportTags
        {
            get { return _importtags; }
            set 
            { 
                _importtags = value;
                NotifyPropertyChanged("ImportTags");
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SelectedItemChangedHandler(IImportTag tag)
        {
            //Get the last selected folder.
            IImportTag lastSelectedTag = _selectedTags.LastOrDefault(); 

            //Unless the ctrl button is pressed, clear any existing selections.
            if (!IsCtrlPressed)
            {
                _selectedTags.ForEach(f => f.IsSelected = false);
                _selectedTags.Clear();
            }

            //If shift button is pressed select everything between the last folder
            //selected and the newly selected folder.
            if (IsShiftPressed)
            {
                SelectTagsWithShiftBehavior(tag, lastSelectedTag);
                return;

            } 

            //Select folder and add it to the collection of selected folders.
            tag.IsSelected = true;
            _selectedTags.Add(tag);
        }

        private void SelectTagsWithShiftBehavior(IImportTag tag, IImportTag lastSelectedTag)
        {
            //Flatten out the folder tree.
            List<IImportTag> flattenedFolders = FlattenTreeStructure(_importtags);
            /*List<IImportTag> flattenedFolders = new List<IImportTag>();

            foreach (var folder in _importtags)
            {
                flattenedFolders.Add(folder);
            }*/

            //Get the index of the selected and previously selected
            //folder in the flattened list.
            /*int startIndex = flattenedFolders.BinarySearch(lastSelectedTag, new TagComparer());
            int endIndex = flattenedFolders.BinarySearch(tag, new TagComparer());*/
            searchID = lastSelectedTag.Id;
            int startIndex = flattenedFolders.FindIndex(FindById);
            searchID = tag.Id;
            int endIndex = flattenedFolders.FindIndex(FindById);

            //swap the values if the start is greater than the end.
            if (startIndex > endIndex)
            {
                var temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
            }

            //iterate and select folders that fall within the range of the
            //previously and newly selected folders.
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                IImportTag stag = flattenedFolders[i];
                stag.IsSelected = true;
                _selectedTags.Add(stag);
            }
        }

        private static int searchID = 0;
        private static bool FindById(IImportTag t)
        {
            return (t.Id == searchID);
        }

        private List<IImportTag> FlattenTreeStructure(IEnumerable<IImportTag> folderList)
        {
            List<IImportTag> flattenedFolders = new List<IImportTag>();
            foreach (var folder in folderList/*.OrderBy(f => f.Name)*/)
            {
                flattenedFolders.Add(folder);
                if (folder.ImportTags != null)
                {
                    var children = FlattenTreeStructure(folder.ImportTags);
                    flattenedFolders.AddRange(children);
                }
            }
            return flattenedFolders;
        }

        private bool IsShiftPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftShift)
                    || Keyboard.IsKeyDown(Key.RightShift);
            }
        }

        private bool IsCtrlPressed
        {
            get
            {
                return Keyboard.IsKeyDown(Key.LeftCtrl)
                    || Keyboard.IsKeyDown(Key.RightCtrl);
            }
        }

        public void FillSomeData()
        {
            
            List<ImportTag> lista = new List<ImportTag>();
            lista.Add(new ImportTag() { Name = "uno", Address = "DB1.DBW0", Id = 0 });

            List<ImportTag> figli = new List<ImportTag>();
            figli.Add(new ImportTag() { Name = "0", Address = "DB?", Id = 3, parentId = 1 });
            figli.Add(new ImportTag() { Name = "1", Address = "DB?", Id = 4, parentId = 1 });
            figli.Add(new ImportTag() { Name = "2", Address = "DB?", Id = 5, parentId = 1 });

            lista.Add(new ImportTag() { Name = "due", Address = "DB1.DBW2", Id = 1, ImportTags = figli});
            lista.Add(new ImportTag() { Name = "tre", Address = "DB1.DBW4", Id = 2 });

            ImportTags = lista;
        }

        public class TagComparer : IComparer<IImportTag>
        {
            public int Compare(IImportTag x, IImportTag y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }
    }
}
