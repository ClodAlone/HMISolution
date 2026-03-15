using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces;
using System.Collections;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    public interface IDropRecordAware
    {
        bool OnDropRecord(object dropObject);
    }
}