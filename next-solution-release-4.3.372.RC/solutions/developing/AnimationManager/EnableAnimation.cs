using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WINDOWS_UWP
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
#endif
#else
using Windows.UI.Xaml.Controls;
#endif
using System.Runtime.Serialization;
#if !NET_STANDARD
using System.Windows;
using Utilities.Animations;
#endif
using System.ComponentModel;

namespace AnimationManager
{
#if !WINDOWS_UWP && !NET_STANDARD
    [TypeConverter(typeof(LocalizedEnumConverter))]
#endif
    public enum EnableCompareModes
    {
        Different,
        Equal,
        Major,
        Minor,
        MajorEqual,
        MinorEqual
    };

    [DataContract(Name = "EnableAnimation")]
    public class EnableAnimation : AnimationManager
    {
        #region Properties
        double compareValue = 0;
        [DataMember]
        public double CompareValue
        {
            get { return compareValue; }
            set
            {
                if (compareValue == value)
                    return;
                compareValue = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("CompareValue");
#endif
                Reexecute();
#endif
            }
        }

        EnableCompareModes compareMode = EnableCompareModes.Different;
        [DataMember]
        public EnableCompareModes CompareMode
        {
            get { return compareMode; }
            set
            {
                if (compareMode == value)
                    return;
                compareMode = value;
#if !NET_STANDARD
#if !WINDOWS_UWP
                OnPropertyChanged("CompareMode");
#endif
                Reexecute();
#endif
            }
        }
        #endregion

        #region Overrides
#if !NET_STANDARD
#if !WINDOWS_UWP
        public override bool this[string propertyName]
        {
            get
            {
                if (propertyName == "AnimationTime" || propertyName == "Repeatable" || 
                    propertyName == "Autoreverse" || propertyName == "AnimationEquation" || 
                    propertyName == "TagMinValue" || propertyName == "TagMaxValue" || propertyName == "AnimationBehavior")
                {
                    return false;
                }

                return base[propertyName];
            }
        }
        public override void Demo()
        {
            Control.IsEnabled = CommonTarget != 0;
            base.Demo();
        }
#endif
        public override void Execute()
        {
            if (Control == null)
                return;

            bool bResult = false;
            switch (CompareMode)
            {
                case EnableCompareModes.Different: bResult = dTargetValue != CompareValue; break;
                case EnableCompareModes.Equal: bResult = dTargetValue == CompareValue; break;
                case EnableCompareModes.Major: bResult = dTargetValue > CompareValue; break;
                case EnableCompareModes.MajorEqual: bResult = dTargetValue >= CompareValue; break;
                case EnableCompareModes.Minor: bResult = dTargetValue < CompareValue; break;
                case EnableCompareModes.MinorEqual: bResult = dTargetValue <= CompareValue; break;
            }

#if !WINDOWS_UWP
            Control.IsEnabled = bResult && !GetIsAccessDenied(Control);
#else
            if (Control is ContentControl)
                (Control as ContentControl).IsEnabled = bResult && !GetIsAccessDenied(Control);
#endif
        }

        public override void Stop()
        {
#if !WINDOWS_UWP
            if (Control != null)
                Control.IsEnabled = true;
#else
            if (Control != null && Control is ContentControl)
                (Control as ContentControl).IsEnabled = false;
#endif
            base.Stop();
        }
#endif
        public override String Name
        {
            get
            {
                return Properties.Resources.EnableName;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public override bool Is2D
        {
            get
            {
                return true;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
        public override String AnimationSummary
        {
            get
            {
                return base.AnimationSummary;
            }
        }

        
#endif

        #endregion
    }
}
