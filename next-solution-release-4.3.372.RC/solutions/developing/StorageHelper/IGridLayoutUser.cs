using System;
using UFInterfaces;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace StorageHelper
{
    public interface IGridLayoutUser 
    {
        List<StorageColumn> GetColumns();
    }
}
