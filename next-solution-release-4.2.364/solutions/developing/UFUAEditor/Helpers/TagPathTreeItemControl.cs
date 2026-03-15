using UFUAModel;
using WPFUtilities;
using System.Windows.Media;
using System.Windows;

namespace TempVariablesManager.Helpers
{
    public class TagPathTreeItemControl : TreeItemControl
    {
        #region DP
        #region FolderPath
        public static readonly DependencyProperty FolderPathProperty = DependencyProperty.Register("FolderPath", typeof(string), typeof(TagPathTreeItemControl), new UIPropertyMetadata(null, new PropertyChangedCallback(OnFolderPathChanged), new CoerceValueCallback(OnCoerceFolderPath)));

        private static object OnCoerceFolderPath(DependencyObject o, object value)
        {
            TagPathTreeItemControl TempVarTreeItemControl = o as TagPathTreeItemControl;
            if (TempVarTreeItemControl != null)
                return TempVarTreeItemControl.OnCoerceFolderPath((string)value);
            else
                return value;
        }

        private static void OnFolderPathChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TagPathTreeItemControl TempVarTreeItemControl = o as TagPathTreeItemControl;
            if (TempVarTreeItemControl != null)
                TempVarTreeItemControl.OnFolderPathChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceFolderPath(string value)
        {
            // TODO: Keep the proposed value within the desired range.
            return value;
        }

        protected virtual void OnFolderPathChanged(string oldValue, string newValue)
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            OnPropertyChanged("FolderPath");
        }

        public string FolderPath
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return (string)GetValue(FolderPathProperty);
            }
            set
            {
                SetValue(FolderPathProperty, value);
            }
        }
        #endregion
        #endregion

        #region Ctor
        public TagPathTreeItemControl(object header, ImageSource icon = null) : base(header, icon)
        {
            if (header.GetType() == typeof(UFUATag))
            {
                var tag = header as UFUATag;
                FolderPath = tag.FolderPath;
            }
        }
        #endregion
    }
}
