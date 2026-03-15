using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces;
using UFUAModel;
using WPFUtilities.Services;
using Utilities;

namespace UFUAEditor.ViewModels
{
    public class EUImportViewModel: BaseImportViewModel<ImportEngineeringUnit>
    {        
        public EUImportViewModel(): base()
        {
        }

        public override void LoadData()
        {
            LoadingDataError = false;
            ErrorMessage = String.Empty;
            CsvImportService<ImportEngineeringUnit, UFUAEngineeringUnitMap> importService = new CsvImportService<ImportEngineeringUnit, UFUAEngineeringUnitMap>();

            try
            {
                ItemsSource = importService.ImportCsv(FilePath, Delimiter);
            }
            catch(Exception ex)
            {
                LoadingDataError = true;
                ErrorMessage = string.Format(Properties.Resources.EU_ImportErrorMessage, FilePath);
            }
        }

        public override List<ImportEngineeringUnit> GetSelectedItems()
        {
            return ItemsSource.FindAll(x => x.GetIsSelected());
        }

        public override List<ImportEngineeringUnit> GetVisibleItems()
        {
            return ItemsSource.FindAll(x => x.GetIsVisible());
        }
        public override void UnselectAll()
        {
            foreach(var item in ItemsSource)
            {
                if (item.GetIsVisible())
                    item.SetIsSelected(false);
            }
        }
        public override void SelectAll()
        {            
            foreach (var item in ItemsSource)
            {
                if(item.GetIsVisible())
                    item.SetIsSelected(true);
            }            
        }        
    }
}
