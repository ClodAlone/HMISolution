using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces;

namespace Utilities
{
    public class BaseImportItem : IImportItem
    {
        protected bool _isSelected;
        protected bool _isVisible = true;
        public bool GetIsSelected()
        {
            return _isSelected;
        }
        public void SetIsSelected(bool value)
        {
            _isSelected = value;
        }

        public bool GetIsVisible()
        {
            return _isVisible;
        }
        public void SetIsVisible(bool value)
        {
            _isVisible = value;
        }
    }
}
