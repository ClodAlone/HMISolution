using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonControls;
using Utilities;
using System.Windows;
using ReportSettings.Documents;

namespace ReportSettings.PropertyDataTemplate
{
    public class DataReaderReference
    {
        #region Declarations
        readonly ReportDocument Document;
        #endregion

        #region Constructors
        public DataReaderReference(ReportDocument doc)
        {
            Document = doc;
        }
        #endregion
       
        #region Methods
        public bool Edit()
        {
            var itemControlSourceProp = new ItemControlSourceProperties()
            {
                DataContext = Document
            };

            itemControlSourceProp.ClearValue(FrameworkElement.WidthProperty);
            itemControlSourceProp.ClearValue(FrameworkElement.HeightProperty);

            GeneralDialogContent Dialog = new GeneralDialogContent(itemControlSourceProp);
            Dialog.HelpLink = "DataReaderEditor";
            return (Dialog.ShowDialog() == true);
        }
        #endregion


    }
}
