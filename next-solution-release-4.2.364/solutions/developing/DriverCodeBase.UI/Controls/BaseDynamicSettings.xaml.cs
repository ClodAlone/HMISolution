using System;
using System.Collections.Generic;
using System.Linq;
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
using OPCUAViewModel;
using System.Windows.Threading;
using Utilities;
using DriverCodeBase.Extensions;
using Utilities.WPF;

namespace DriverCodeBase.UI.Controls
{
    /// <summary>
    /// Interaction logic for BaseDynamicSettings.xaml
    /// </summary>
    public partial class BaseDynamicSettings : UserControl
    {
        /// <summary>   true if the data was loaded. </summary>
        bool bLoaded;

        /// <summary>
        /// basic dynamic tag settings 
        /// </summary>
        public BaseDynamicSettings()
        {
            InitializeComponent();

            Loaded += (o, e) =>
            {
                if (bLoaded)
                    return;

                bLoaded = true;

                DynTagSettings dts = DataContext as DynTagSettings;
                if (dts != null)
                {
                    // force to select 1st station when no station was previously associated to tag (generally when create a new tag)
                    if (string.IsNullOrEmpty(dts.StationName) && CmbStation.Items.Count == 1)
                    {
                        dts.StationName = ((StationSettings)CmbStation.Items[0]).Name;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dts.StationName))
                        {
                            // check if station assigned to tag still exist --> if not, empty it
                            if (CmbStation.Items.Cast<StationSettings>().ToList().Count(a => a.Name == dts.StationName) == 0)
                                dts.StationName = String.Empty;
                        }
                    }
                }
            };
        }

        private void SetConditionalTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as DynTagSettings;
            Opc.Ua.NodeId nodeId;
            if (!NodeIdHelper.TryParse(nDC.ConditionalVariableId, out nodeId))
                nodeId = Opc.Ua.NodeId.Null;
            var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.ConditionalVariableName, nodeId);
            var tag = tagEntityReference.Edit(this.FindParent<Window>());
            if (tag != null)
            {
                nDC.ConditionalVariableId = tag.NodeId.ToString();
                nDC.ConditionalVariableName = tag.ToString();
            }

        }

        private void ResetConditionalTag_Click(object sender, RoutedEventArgs e)
        {
            var nDC = DataContext as DynTagSettings;
            nDC.ConditionalVariableName = null;
            nDC.ConditionalVariableId = null;
        }

        public void SetOffsetVariableVisible(bool visible = true, string caption = null)
        {
            if (visible)
            {
                lblOffsetVariable.Visibility = Visibility.Visible;
                if (!string.IsNullOrEmpty(caption))
                    lblOffsetVariable.Text = caption;
                DKOffsetVariable.Visibility = Visibility.Visible;
            }
            else
            {
                lblOffsetVariable.Visibility = Visibility.Collapsed;
                DKOffsetVariable.Visibility = Visibility.Collapsed;
            }
        }

        private void btnOffsetVariableCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var nDC = DataContext as DynTagSettings;
            nDC.OffsetVariableName = null;
            nDC.OffsetVariableId = null;
        }

        private void btnOffsetVariableBrowse_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var nDC = DataContext as DynTagSettings;
            Opc.Ua.NodeId nodeId;
            if (!NodeIdHelper.TryParse(nDC.OffsetVariableId, out nodeId))
                nodeId = Opc.Ua.NodeId.Null;
            var tagEntityReference = new UFUAModel.TagEntityReference(Guid.Empty, nDC.OffsetVariableName, nodeId);
            var tag = tagEntityReference.Edit(this.FindParent<Window>());
            if (tag != null)
            {
                nDC.OffsetVariableId = tag.NodeId.ToString();
                nDC.OffsetVariableName = tag.ToString();
            }
        }
    }
}
