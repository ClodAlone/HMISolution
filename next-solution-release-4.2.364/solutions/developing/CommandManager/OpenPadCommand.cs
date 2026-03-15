using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using OPCUAViewModel;
#if !NET_STANDARD
using Utilities.WPF;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities.Converters;
using UFUAEditor.ComponentService;
using AuditTrace;
#endif
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Controls;
using System.Windows.Threading;
using Converters;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif
using UFInterfaces;
using Utilities;
using DocumentManager.ComponentService;
using System.Windows;
using System.Globalization;
using Opc.Ua;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using Utilities.Converters;
using UFUAServerInfo;
using ViewModelLib;

namespace CommandManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum OpenPadCommandType
    {
        NumericPad,
        AlphaNumericPad
    }

    [DataContract(Name = "OpenPadCommand", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OpenPadCommand : ValueCommand
    {

        [DataMember]
        public override ValueCommandType Type
        {
            get
            {
                return (openpadtype == OpenPadCommandType.AlphaNumericPad ? ValueCommandType.AlphaNumericPad : ValueCommandType.NumericPad);
            }
        }

        [DataMember]
        OpenPadCommandType openpadtype;
        public OpenPadCommandType OpenPadType
        {
            get
            {
                return openpadtype;
            }
            set
            {
                if (openpadtype == value)
                    return;
                openpadtype = value;
                MinValue = MaxValue = Decimals = 0;
#if !WINDOWS_UWP && !NET_STANDARD
                OnPropertyChanged("OpenPadType");
                OnPropertyChanged("CommandSummary");
                OnPropertyVisiblityChanged("OpenPadType");
#endif
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "Type")
                    return false;
                else if (propertyName == "X" || propertyName == "Y" || propertyName == "IsRelative")
                {
                    return OpenPadType == OpenPadCommandType.NumericPad || OpenPadType == OpenPadCommandType.AlphaNumericPad;
                }
                return base[propertyName];
            }
        }
#endif

        public override String Name
        {
            get
            {
                return Properties.Resources.OpenPadName;
            }
        }

        public override bool IsUICommand()
        {
            return true;
        }
    }
}
