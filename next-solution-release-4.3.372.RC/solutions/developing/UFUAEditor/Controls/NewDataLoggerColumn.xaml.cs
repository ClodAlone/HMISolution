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
using Utilities;
using Utilities.WPF;
using CommonControls;
using UFUAEditor.Document;
using UFInterfaces.Editors;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewDataLoggerColumn.xaml
    /// </summary>
    public partial class NewDataLoggerColumn : UserControl
    {
        #region Declarations
        UFUAServerDocument ufuaDocument;
        #endregion

        #region Construtctors
        public NewDataLoggerColumn(UFUAServerDocument doc)
        {
            InitializeComponent();

            Loaded += (o, e) => 
            {
                var dlc = DataContext as DataLoggerModel.DataLoggerColumn;
                if (dlc != null)
                {
                    TagEntityReferenceModel Tag = new TagEntityReferenceModel() { Value = dlc.ColumnTag };

                    textColumnTag.DataContext = Tag;

                    Tag.PropertyChanged += (obj, ea) =>
                    {
                        if (ea.PropertyName == "Value")
                        {
                            try
                            {
                                if (Tag.Value != null)
                                    dlc.ColumnTag = Tag.Value;
                                else
                                    dlc.ColumnTag = null;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    };
                }
            };
            
            ufuaDocument = doc;
        }
        #endregion

    }
}
