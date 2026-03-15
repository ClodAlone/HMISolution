using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Mindscape.WpfElements;
using UFUAEditor.Document;
using Utilities;

namespace UFUAEditor.Controls
{
    /// <summary>
    /// Interaction logic for NewAlarmDefinition.xaml
    /// </summary>
    public partial class NewAlarmDefinition : UserControl
    {
        #region Declarations
        readonly UFUAServerDocument Document;
        readonly bool IsAlarmDefinition;
        #endregion

        public NewAlarmDefinition(UFUAServerDocument doc, bool alarm = true)
        {
            InitializeComponent();
            Document = doc;
            IsAlarmDefinition = alarm;

            Loaded += (s, e) => 
            {
                var tag = DataContext as UFUAModel.UFUAAlarmDefinition;
                if (!IsAlarmDefinition && tag != null)
                {
                    tag.Severity = 0;
                    textBlockSeverity.Visibility = textSeverity.Visibility = tag.Severity > 0 ? Visibility.Visible : Visibility.Collapsed;
                }

                CheckVisibilityOnSeverity();
            };
            
            comboAlarmType.ItemsSource = Enum.GetValues(typeof(UFUAModel.AlarmType));
            comboConditionType.ItemsSource = Enum.GetValues(typeof(UFUAModel.ConditionType));
            comboDeviationType.ItemsSource = Enum.GetValues(typeof(UFUAModel.DeviationType));

            textEditDelayTimeOn.timeSpanControl.SetBinding(TimeSpanPicker.SelectedTimeSpanProperty,
                new Binding("DelayTimeOn")
                {
                    ValidatesOnDataErrors = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
            textEditDelayTimeOff.timeSpanControl.SetBinding(TimeSpanPicker.SelectedTimeSpanProperty,
                new Binding("DelayTimeOff")
                {
                    ValidatesOnDataErrors = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
            textEditTimeUnit.timeSpanControl.SetBinding(TimeSpanPicker.SelectedTimeSpanProperty,
                new Binding("TimeUnit")
                {
                    ValidatesOnDataErrors = true,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay
                });
        }

        private void comboAlarmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = DataContext as UFUAModel.UFUAAlarmDefinition;
            //if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveLimit || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveLimit)
            //{
            //    textDeviationType.Visibility = Visibility.Collapsed;
            //    comboDeviationType.Visibility = Visibility.Collapsed;

            //    textConditionType.Visibility = Visibility.Collapsed;
            //    comboConditionType.Visibility = Visibility.Collapsed;
            //    textBlockActivationValue.Visibility = Visibility.Collapsed;
            //    textActivationValue.Visibility = Visibility.Collapsed;

            //    textBlockLowLowLimit.Visibility = Visibility.Visible;
            //    textBlockLowLimit.Visibility = Visibility.Visible;
            //    textBlockHighLimit.Visibility = Visibility.Visible;
            //    textBlockHighHighLimit.Visibility = Visibility.Visible;

            //    textBlockLowLowLevel.Visibility = Visibility.Collapsed;
            //    textBlockLowLevel.Visibility = Visibility.Collapsed;
            //    textBlockHighLevel.Visibility = Visibility.Collapsed;
            //    textBlockHighHighLevel.Visibility = Visibility.Collapsed;

            //    textLowLowLimit.Visibility = Visibility.Visible;
            //    textLowLimit.Visibility = Visibility.Visible;
            //    textHighLimit.Visibility = Visibility.Visible;
            //    textHighHighLimit.Visibility = Visibility.Visible;
            //}
            //else 
            if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveLevel || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveLevel)
            {
                textDeviationType.Visibility = Visibility.Collapsed;
                comboDeviationType.Visibility = Visibility.Collapsed;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;
                textBlockActivationLowValue.Visibility = Visibility.Collapsed;
                textActivationLowValue.Visibility = Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLimit.Visibility = Visibility.Collapsed;
                textBlockHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighHighLimit.Visibility = Visibility.Collapsed;

                textBlockLowLowLevel.Visibility = Visibility.Visible;
                textBlockLowLevel.Visibility = Visibility.Visible;
                textBlockHighLevel.Visibility = Visibility.Visible;
                textBlockHighHighLevel.Visibility = Visibility.Visible;

                gridLowLowLimit.Visibility = Visibility.Visible;
                gridLowLimit.Visibility = Visibility.Visible;
                gridHighLimit.Visibility = Visibility.Visible;
                gridHighHighLimit.Visibility = Visibility.Visible;

                textLowLowLimit.Visibility = Visibility.Visible;
                textLowLimit.Visibility = Visibility.Visible;
                textHighLimit.Visibility = Visibility.Visible;
                textHighHighLimit.Visibility = Visibility.Visible;

                textHighPercentLimit.Visibility = Visibility.Collapsed;
                textHighPercentLimit.Visibility = Visibility.Collapsed;
                textHighHighPercentLimit.Visibility = Visibility.Collapsed;
                textLowPercentLimit.Visibility = Visibility.Collapsed;
                textLowLowPercentLimit.Visibility = Visibility.Collapsed;

                textBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;

                textBlockDelayOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;
                
                textBlockDelayOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;
            }
            else if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveDeviation || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveDeviation)
            {
                textDeviationType.Visibility = Visibility.Visible;
                comboDeviationType.Visibility = Visibility.Visible;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;
                textBlockActivationLowValue.Visibility = Visibility.Collapsed;
                textActivationLowValue.Visibility = Visibility.Collapsed;

                gridLowLowLimit.Visibility = Visibility.Visible;
                gridLowLimit.Visibility = Visibility.Visible;
                gridHighLimit.Visibility = Visibility.Visible;
                gridHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLimit.Visibility = Visibility.Visible;
                textBlockLowLimit.Visibility = Visibility.Visible;
                textBlockHighLimit.Visibility = Visibility.Visible;
                textBlockHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                textBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;

                textBlockDelayOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;

                textBlockDelayOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;

                CheckCurrencySymbol();
            }
            else if (tag.AlarmType == UFUAModel.AlarmType.ExclusiveRateOfChange || tag.AlarmType == UFUAModel.AlarmType.NonExclusiveRateOfChange)
            {
                textDeviationType.Visibility = Visibility.Visible;
                comboDeviationType.Visibility = Visibility.Visible;

                textConditionType.Visibility = Visibility.Collapsed;
                comboConditionType.Visibility = Visibility.Collapsed;
                textBlockActivationValue.Visibility = Visibility.Collapsed;
                textActivationValue.Visibility = Visibility.Collapsed;
                textBlockActivationLowValue.Visibility = Visibility.Collapsed;
                textActivationLowValue.Visibility = Visibility.Collapsed;

                gridLowLowLimit.Visibility = Visibility.Visible;
                gridLowLimit.Visibility = Visibility.Visible;
                gridHighLimit.Visibility = Visibility.Visible;
                gridHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLimit.Visibility = Visibility.Visible;
                textBlockLowLimit.Visibility = Visibility.Visible;
                textBlockHighLimit.Visibility = Visibility.Visible;
                textBlockHighHighLimit.Visibility = Visibility.Visible;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                textBlockTimeUnit.Visibility = Visibility.Visible;
                textEditTimeUnit.Visibility = Visibility.Visible;

                textBlockDelayOn.Visibility = Visibility.Collapsed;
                textEditDelayTimeOn.Visibility = Visibility.Collapsed;

                textBlockDelayOff.Visibility = Visibility.Collapsed;
                textEditDelayTimeOff.Visibility = Visibility.Collapsed;

                CheckCurrencySymbol();
            }
            else if (tag.AlarmType == UFUAModel.AlarmType.TripAlarm)
            {
                textDeviationType.Visibility = Visibility.Collapsed;
                comboDeviationType.Visibility = Visibility.Collapsed;

                textConditionType.Visibility = Visibility.Visible;
                comboConditionType.Visibility = Visibility.Visible;
                textBlockActivationValue.Visibility = Visibility.Visible;
                textActivationValue.Visibility = Visibility.Visible;
                textBlockActivationLowValue.Visibility = tag.ConditionType == UFUAModel.ConditionType.Between ? Visibility.Visible : Visibility.Collapsed;
                textActivationLowValue.Visibility = tag.ConditionType == UFUAModel.ConditionType.Between ? Visibility.Visible : Visibility.Collapsed;

                textBlockLowLowLimit.Visibility = Visibility.Collapsed;
                textBlockLowLimit.Visibility = Visibility.Collapsed;
                textBlockHighLimit.Visibility = Visibility.Collapsed;
                textBlockHighHighLimit.Visibility = Visibility.Collapsed;

                textBlockLowLowLevel.Visibility = Visibility.Collapsed;
                textBlockLowLevel.Visibility = Visibility.Collapsed;
                textBlockHighLevel.Visibility = Visibility.Collapsed;
                textBlockHighHighLevel.Visibility = Visibility.Collapsed;

                gridLowLowLimit.Visibility = Visibility.Collapsed;
                gridLowLimit.Visibility = Visibility.Collapsed;
                gridHighLimit.Visibility = Visibility.Collapsed;
                gridHighHighLimit.Visibility = Visibility.Collapsed;

                textLowLowLimit.Visibility = Visibility.Collapsed;
                textLowLimit.Visibility = Visibility.Collapsed;
                textHighLimit.Visibility = Visibility.Collapsed;
                textHighHighLimit.Visibility = Visibility.Collapsed;

                textHighPercentLimit.Visibility = Visibility.Collapsed;
                textHighPercentLimit.Visibility = Visibility.Collapsed;
                textHighHighPercentLimit.Visibility = Visibility.Collapsed;
                textLowPercentLimit.Visibility = Visibility.Collapsed;
                textLowLowPercentLimit.Visibility = Visibility.Collapsed;

                textBlockTimeUnit.Visibility = Visibility.Collapsed;
                textEditTimeUnit.Visibility = Visibility.Collapsed;

                textBlockDelayOn.Visibility = Visibility.Visible;
                textEditDelayTimeOn.Visibility = Visibility.Visible;

                textBlockDelayOff.Visibility = Visibility.Visible;
                textEditDelayTimeOff.Visibility = Visibility.Visible;
            }
        }

        private void comboConditionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = DataContext as UFUAModel.UFUAAlarmDefinition;
            if (tag.AlarmType == UFUAModel.AlarmType.TripAlarm && tag.ConditionType == UFUAModel.ConditionType.Between)
            {
                textBlockActivationLowValue.Visibility = Visibility.Visible;
                textActivationLowValue.Visibility = Visibility.Visible;
            }
            else
            {
                textBlockActivationLowValue.Visibility = Visibility.Collapsed;
                textActivationLowValue.Visibility = Visibility.Collapsed;
            }
        }

        private void textSeverity_ValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            CheckVisibilityOnSeverity();
        }

        void CheckVisibilityOnSeverity()
        {
            var tag = DataContext as UFUAModel.UFUAAlarmDefinition;
            if (tag != null)
            {
                textBlockSupportAck.Visibility = checkBoxSupportAck.Visibility = tag.Severity > 0 ? Visibility.Visible : Visibility.Collapsed;
                textBlockSupportReset.Visibility = checkBoxSupportReset.Visibility = tag.Severity > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void comboDeviationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckCurrencySymbol();
        }

        void CheckCurrencySymbol()
        {
            var tag = DataContext as UFUAModel.UFUAAlarmDefinition;

            if (tag.AlarmType != UFUAModel.AlarmType.ExclusiveDeviation &&
                tag.AlarmType != UFUAModel.AlarmType.NonExclusiveDeviation &&
                tag.AlarmType != UFUAModel.AlarmType.ExclusiveRateOfChange &&
                tag.AlarmType != UFUAModel.AlarmType.NonExclusiveRateOfChange)
                return;

            bool percent = tag.DeviationType == UFUAModel.DeviationType.PercentOfEURange ||
                            tag.DeviationType == UFUAModel.DeviationType.PercentOfRange ||
                            tag.DeviationType == UFUAModel.DeviationType.PercentOfValue;

            textHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textHighHighPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textLowPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            textLowLowPercentLimit.Visibility = percent ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            textHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textHighHighLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textLowLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            textLowLowLimit.Visibility = percent ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
