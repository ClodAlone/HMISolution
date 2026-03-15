using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces
{
    public interface IImportItem
    {
        bool GetIsSelected();
        void SetIsSelected(bool isSelected);
        bool GetIsVisible();
        void SetIsVisible(bool isVisible);
    }
}
