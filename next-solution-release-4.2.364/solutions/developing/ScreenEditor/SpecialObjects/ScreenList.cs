using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.WindowsUI;
using ScreenManager.ComponentService;
using ScreenSettings;

namespace ScreenManager.SpecialObjects
{
    [CollectionDataContract(
            Name = "ScreenList",
            ItemName = "Screen")]
    [Serializable]
    public class ScreenList : List<String>
    {
    }
}
