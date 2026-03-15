using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace FanucCNC.UI
{
    public partial class DynamicSettingsControl_cnc_pdf_rdactpt : UserControl, IDisposable
    {        
        public DynamicSettingsControl_cnc_pdf_rdactpt()
        {
            InitializeComponent();

            bool bLoaded = false;
            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;
                bLoaded = true;

                List<FanucCNCDynTag_cnc_pdf_rdactpt.FC> lst = new List<FanucCNCDynTag_cnc_pdf_rdactpt.FC>();
                lst.Add(new FanucCNCDynTag_cnc_pdf_rdactpt.FC(FanucCNCDynTag_cnc_pdf_rdactpt.ProgramElement.Name, FanucCNCDynTag_cnc_pdf_rdactpt.ProgramElement.Name.ToString()));
                lst.Add(new FanucCNCDynTag_cnc_pdf_rdactpt.FC(FanucCNCDynTag_cnc_pdf_rdactpt.ProgramElement.Pointer, FanucCNCDynTag_cnc_pdf_rdactpt.ProgramElement.Pointer.ToString()));
                cmbProgramPart.ItemsSource = lst;
            };            
        }       

        #region IDisposable Members

        public void Dispose()
        {
            
        }
        #endregion
    }
}
