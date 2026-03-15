using System.Windows.Controls;

namespace FanucCNC.UI
{
    public interface IDynamicSettingsChildrenControlRefresh
    {
        void interfaceDataChanged(FanucCNCProtocol.MachineSeries machineSerie); //, short cncPath);
    }
}
