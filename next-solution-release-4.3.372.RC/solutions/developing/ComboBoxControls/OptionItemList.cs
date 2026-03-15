using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;

namespace ComboBoxControls
{
    public class OptionItemList : ObservableCollection<OptionItem>
    {
        #region Constructors
        public OptionItemList() 
        { }

        public OptionItemList(OptionItemList instance)
        {
            if (instance == null)
                return;

            foreach (var item in instance)
                Add(new OptionItem(item));
        }
        #endregion
    }
}
