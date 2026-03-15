using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Diagram.Core;

namespace ReportManager
{
    internal class RecoverSelectionHelper : IDisposable
    {
        #region Declarations
        readonly DiagramItem primarySelection;
        readonly List<DiagramItem> selectedItems;
        readonly XRDiagramControl diagram;
        #endregion

        #region Constructors
        public RecoverSelectionHelper(XRDiagramControl diagram)
        {
            this.diagram = diagram;
            if (diagram != null)
            {
                primarySelection = diagram.PrimarySelection;
                if (diagram.SelectedItems != null)
                    selectedItems = diagram.SelectedItems.ToList();
            }
        }
        #endregion

        #region Properties
        public DiagramItem PrimarySelection
        {
            get
            {
                return primarySelection;
            }
        }

        public IList<DiagramItem> SelectedItems
        {
            get
            {
                return selectedItems;
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (SelectedItems != null)
            {
                diagram.ClearSelection();
                diagram.SelectItems(SelectedItems);
                // diagram.Selection().UpdateSelectionAdorners(); 16.2 missing
            }
        }
        #endregion
    }
}
