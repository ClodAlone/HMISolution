// See https://aka.ms/new-console-template for more information
using System;
using System.Drawing;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

/*

sensor:
  - platform: rest
    name: QCells Cloud
    resource: !secret qcells_cloud
    method: GET
    scan_interval: 300
    timeout: 60
    value_template: 'OK'
    json_attributes_path: "$.result"
    json_attributes:
      - sn
      - acpower
      - yieldtoday
      - yieldtotal
      - feedinpower
      - feedinenergy
      - consumeenergy
      - feedinpowerM2
      - soc
      - peps1
      - peps2
      - peps3
      - inverterType
      - inverterStatus
      - uploadTime
      - batPower
      - powerdc1
      - powerdc2
      - powerdc3
      - powerdc4
      - batStatus
  - platform: template
    sensors:
      qcells_cloud_sn:
        friendly_name: "QCells Unique identifier of communication module"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'sn') }}"
      qcells_cloud_acpower:
        friendly_name: "QCells Inverter AC power total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'acpower') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_yieldtoday:
        friendly_name: "QCells Inverter AC energy out daily"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'yieldtoday') }}"
        unit_of_measurement: "kWh"
        device_class: energy
      qcells_cloud_yieldtotal:
        friendly_name: "QCells Inverter AC energy out total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'yieldtotal') }}"
        unit_of_measurement: "kWh"
        device_class: energy
      qcells_cloud_feedinpower:
        friendly_name: "QCells Grid power total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'feedinpower') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_feedinenergy:
        friendly_name: "QCells Grid energy to Grid total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'feedinenergy') }}"
        unit_of_measurement: "kWh"
        device_class: energy
      qcells_cloud_consumeenergy:
        friendly_name: "QCells Grid energy from Grid total "
        value_template: "{{ state_attr('sensor.qcells_cloud', 'consumeenergy') }}"
        unit_of_measurement: "kWh"
        device_class: energy
      qcells_cloud_feedinpowerm2:
        friendly_name: "QCells Inverter Meter2 AC power total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'feedinpowerM2') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_soc:
        friendly_name: "QCells BMS energy SOC"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'soc') }}"
        unit_of_measurement: "%"
      qcells_cloud_peps1:
        friendly_name: "QCells Inverter AC EPS power 1"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'peps1') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_peps2:
        friendly_name: "QCells Inverter AC EPS power 2"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'peps2') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_peps3:
        friendly_name: "QCells Inverter AC EPS power 3"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'peps3') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_invertertype:
        friendly_name: "QCells Inverter Type"
        value_template: >-
          {% if state_attr('sensor.qcells_cloud', 'inverterType') == "1" %}
            X1-LX
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "2" %}
            X-Hybrid
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "3" %}
            X1-Hybiyd/Fit
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "4" %}
            X1-Boost/Air/Mini
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "5" %}
            X3-Hybiyd/Fit
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "6" %}
            X3-20K/30K
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "7" %}
            X3-MIC/PRO
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "8" %}
            X1-Smart
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "9" %}
            X1-AC
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "10" %}
            A1-Hybrid
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "11" %}
            A1-Fit
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "12" %}
            A1-Grid
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "13" %}
            J1-ESS
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "14" %}
            X3-Hybrid-G4
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "15" %}
            X1-Hybrid-G4
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "16" %}
            X3-MIC/PRO-G2
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "17" %}
            X1-SPT
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "18" %}
            X1-Boost/Mini-G4
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "19" %}
            A1-HYB-G2
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "20" %}
            A1-AC-G2
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "21" %}
            A1-SMT-G2
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "22" %}
            X3-FTH
          {% elif state_attr('sensor.qcells_cloud', 'inverterType') == "23" %}
            X3-MGA-G2
          {% else %}
            Unknown
          {% endif %}
      qcells_cloud_inverterstatus:
        friendly_name: "QCells Inverter status"
        value_template: >-
          {% if state_attr('sensor.qcells_cloud', 'inverterStatus') == "100" %}
            Wait Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "101" %}
            Check Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "102" %}
            Normal Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "103" %}
            Fault Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "104" %}
            Permanent Fault Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "105" %}
            Update Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "106" %}
            EPS Check Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "107" %}
            EPS Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "108" %}
            Self-Test Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "109" %}
            Idle Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "110" %}
            Standby Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "111" %}
            Pv Wake Up Bat Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "112" %}
            Gen Check Mode
          {% elif state_attr('sensor.qcells_cloud', 'inverterStatus') == "113" %}
            Gen Run Mode
          {% else %}
            Unknown status
          {% endif %}
      qcells_cloud_uploadtime:
        friendly_name: "QCells Update time"
        value_template: "{{ as_timestamp(state_attr('sensor.qcells_cloud', 'uploadTime')) | timestamp_local }}"
        device_class: timestamp
      qcells_cloud_batpower:
        friendly_name: "QCells Inverter DC Battery power total"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'batPower') }}"
        unit_of_measurement: "W"
        device_class: battery
      qcells_cloud_powerdc1 :
        friendly_name: "QCells Inverter DC PV power MPPT1"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'powerdc1') }}"
        device_class: power
        unit_of_measurement: "W"
      qcells_cloud_powerdc2:
        friendly_name: "QCells Inverter DC PV power MPPT2"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'powerdc2') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_powerdc3:
        friendly_name: "QCells Inverter DC PV power MPPT3"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'powerdc3') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_powerdc4:
        friendly_name: "QCells Inverter DC PV power MPPT4"
        value_template: "{{ state_attr('sensor.qcells_cloud', 'powerdc4') }}"
        unit_of_measurement: "W"
        device_class: power
      qcells_cloud_batstatus:
        friendly_name: "QCells BMS Status"
        value_template: >-
          {% if state_attr('sensor.qcells_cloud', 'batStatus') == "0" %}
            Self Use Mode
          {% elif state_attr('sensor.qcells_cloud', 'batStatus') == "1" %}
            Force Time Use
          {% elif state_attr('sensor.qcells_cloud', 'batStatus') == "2" %}
            Back Up Mode
          {% elif state_attr('sensor.qcells_cloud', 'batStatus') == "3" %}
            Feed-in Priority
          {% else %}
            Unknown status
          {% endif %}
# Adding all possible solar strings to one value
      qcells_cloud_powerdc_sum:
        friendly_name: "QCells Inverter DC PV power sum"
        value_template: "{{ states('sensor.qcells_cloud_powerdc1') | float(default=0) + states('sensor.qcells_cloud_powerdc2') | float(default=0) + states('sensor.qcells_cloud_powerdc3') | float(default=0) + states('sensor.qcells_cloud_powerdc4') | float(default=0) }}"
        unit_of_measurement: "W"
        device_class: power
# Calculate house load because this is not part of the Cloud result
      qcells_cloud_house_load:
        friendly_name: QCells Cloud House Load
        value_template: "{{ states('sensor.qcells_cloud_feedinpowerm2') | float(default=0) + states('sensor.qcells_cloud_acpower') | float(default=0) - states('sensor.qcells_cloud_feedinpow
er') | float(default=0) }}"
        unit_of_measurement: W
        device_class: power
# Battery is being charged
      qcells_cloud_battery_charge:
        friendly_name: QCells Cloud Battery Charge
        value_template: >
          {% if states('sensor.qcells_cloud_batpower') | float(default=0) > 0 %}
            {{ states('sensor.qcells_cloud_batpower') | float(default=0) }}
          {% else %}
            0
          {% endif %}
        unit_of_measurement: W
        device_class: power
# Battery is being discharged
      qcells_cloud_battery_discharge:
        friendly_name: QCells Cloud Battery Discharge
        value_template: >
          {% if states('sensor.qcells_cloud_batpower') | float(default=0) < 0 %}
            {{ states('sensor.qcells_cloud_batpower') | float(default=0) | abs }}
          {% else %}
            0
          {% endif %}
        unit_of_measurement: W
        device_class: power
# Feed to grid
      qcells_cloud_grid_feed:
        friendly_name: QCells Cloud Grid Feed
        value_template: >
          {% if states('sensor.qcells_cloud_feedinpower') | float(default=0) > 0 %}
            {{ states('sensor.qcells_cloud_feedinpower') | float(default=0) }}
          {% else %}
            0
          {% endif %}
        unit_of_measurement: W
        device_class: power
# Load from grid
      qcells_cloud_grid_load:
        friendly_name: QCells Cloud Grid Load
        value_template: >
          {% if states('sensor.qcells_cloud_feedinpower') | float(default=0) < 0 %}
            {{ states('sensor.qcells_cloud_feedinpower') | float(default=0) | abs }}
          {% else %}
            0
          {% endif %}
        unit_of_measurement: W
        device_class: power
# Q-Save 9.1 --> 8300W usable minus 10% minimum load multiplied by soc
# E.g. 35% soc --> 2614.5 W
      qcells_cloud_battery_usable:
        friendly_name: QCells Cloud Battery usable
        value_template: "{{ 8300 * (max(states('sensor.qcells_cloud_soc') | float(default=0) - 10, 0) / 100) }}"
        unit_of_measurement: W
        device_class: power
# Solar plus usable battery added
      qcells_cloud_solar_plus_battery_total:
        friendly_name: QCells Cloud Solar plus Battery total
        value_template: "{{ states('sensor.qcells_cloud_powerdc_sum') | float + states('sensor.qcells_cloud_battery_usable') | float }}"
        unit_of_measurement: W
        device_class: power
*/

public class QCellRealtimeInfo
{
    public bool success { get; set; }
    public string exception { get; set; } // UNIX timestamp, typically long or DateTime

    public QCellData result { get; set; }

    public int code { get; set; } // UNIX timestamp, typically long or DateTime
}

public class QCellData
{
    public String inverterSN { get; set; }
    public String sn { get; set; }
    public double acpower { get; set; }
    public double yieldtoday { get; set; }
    public double yieldtotal { get; set; }
    public double feedinpower { get; set; }
    public double feedinenergy { get; set; }
    public double consumeenergy { get; set; }
    public double feedinpowerM2 { get; set; }
    public double soc { get; set; }
    public double peps1 { get; set; }
    public double peps2 { get; set; }
    public double peps3 { get; set; }
    public String inverterType { get; set; }
    public String inverterStatus { get; set; }
    public String uploadTime { get; set; }
    public double batPower { get; set; }
    public double powerdc1 { get; set; }
    public double powerdc2 { get; set; }
    public String powerdc3 { get; set; }
    public String powerdc4 { get; set; }
    public String batStatus { get; set; }
    public String utcDateTime { get; set; }


    public String QCellsBMSStatus { get; set; }
    public String QCellsInverterType { get; set; }
    public String QCellsInverterStatus { get; set; }

    public double QCellsInverterACPowerTotalW { get; set; }
    public double QCellsInverterACEnergyOutDailyKWh { get; set; }
    public double QCellsInverterACEnergyOutTotalKWh { get; set; }
    public double QCellsGridPowerTotalW { get; set; }
    public double QCellsGridEnergyToGridTotalKWh { get; set; }
    public double QCellsGridEnergyFromGridTotalKWh { get; set; }
    public double QCellsInverterMeter2ACPowerTotalW { get; set; }
    public double QCellsBMSEnergySOCPercent { get; set; }
    public double QCellsInverterDCBatteryPowerTotalW { get; set; }
    public double QCellsCloudHouseLoadW { get; set; }
    public bool QCellsCloudBatteryCharge { get; set; }
    public bool QCellsCloudBatteryDischarge { get; set; }
    public bool QCellsCloudGridFeed { get; set; }
    public bool QCellsCloudGridLoad { get; set; }
    public double QCellsCloudBatteryUsableW { get; set; }

    
    internal void NormalizeValues()
    {
        QCellsInverterACPowerTotalW = acpower;
        QCellsInverterACEnergyOutDailyKWh = yieldtoday;
        QCellsInverterACEnergyOutTotalKWh = yieldtotal;
        QCellsGridPowerTotalW = feedinpower;
        QCellsGridEnergyToGridTotalKWh = feedinenergy;
        QCellsGridEnergyFromGridTotalKWh = consumeenergy;
        QCellsInverterMeter2ACPowerTotalW = feedinpowerM2;
        QCellsBMSEnergySOCPercent = soc;
        QCellsInverterDCBatteryPowerTotalW = batPower;
        QCellsCloudHouseLoadW = feedinpowerM2 + acpower - feedinpower;
        QCellsCloudBatteryCharge = batPower > 0;
        QCellsCloudBatteryDischarge = batPower < 0;
        QCellsCloudGridFeed = feedinpower > 0;
        QCellsCloudGridLoad = feedinpower < 0;
        QCellsCloudBatteryUsableW = (8300 * (soc - 10)) / 100;

        switch (batStatus?.ToLower())
        {
            case "0":
                QCellsBMSStatus = "Self Use Mode";
                break;
            case "1":
                QCellsBMSStatus = "Force Time Use";
                break;
            case "2":
                QCellsBMSStatus = "Back Up Mode";
                break;
            case "3":
                QCellsBMSStatus = "Feed -in Priority";
                break;
            default:
                QCellsBMSStatus = "Unknown status";
                break;
        }

        switch (inverterType?.ToLower())
        {
            case "1":
                QCellsInverterType = "X1 - LX";
                break;
            case "2":
                QCellsInverterType = "X - Hybrid";
                break;
            case "3":
                QCellsInverterType = "X1 - Hybiyd / Fit";
                break;
            case "4":
                QCellsInverterType = "X1 - Boost / Air / Mini";
                break;
            case "5":
                QCellsInverterType = "X3 - Hybiyd / Fit";
                break;
            case "6":
                QCellsInverterType = "X3 - 20K / 30K";
                break;
            case "7":
                QCellsInverterType = "X3 - MIC / PRO";
                break;
            case "8":
                QCellsInverterType = "X1 - Smart";
                break;
            case "9":
                QCellsInverterType = "X1 - AC";
                break;
            case "10":
                QCellsInverterType = "A1 - Hybrid";
                break;
            case "11":
                QCellsInverterType = "A1 - Fit";
                break;
            case "12":
                QCellsInverterType = "A1 - Grid";
                break;
            case "13":
                QCellsInverterType = "J1 - ESS";
                break;
            case "14":
                QCellsInverterType = "X3 - Hybrid - G4";
                break;
            case "15":
                QCellsInverterType = "X1 - Hybrid - G4";
                break;
            case "16":
                QCellsInverterType = "X3 - MIC / PRO - G2";
                break;
            case "17":
                QCellsInverterType = "X1 - SPT";
                break;
            case "18":
                QCellsInverterType = "X1 - Boost / Mini - G4";
                break;
            case "19":
                QCellsInverterType = "A1 - HYB - G2";
                break;
            case "20":
                QCellsInverterType = "A1 - AC - G2";
                break;
            case "21":
                QCellsInverterType = "A1 - SMT - G2";
                break;
            case "22":
                QCellsInverterType = "X3 - FTH";
                break;
            case "23":
                QCellsInverterType = "X3 - MGA - G2";
                break;
            default:
                QCellsInverterType = "Unknown status";
                break;
        }

        switch (inverterStatus?.ToLower())
        {
            case "100":
                QCellsInverterStatus = "Wait Mode";
                break;
            case "101":
                QCellsInverterStatus = "Check Mode";
                break;
            case "102":
                QCellsInverterStatus = "Normal Mode";
                break;
            case "103":
                QCellsInverterStatus = "Fault Mode";
                break;
            case "104":
                QCellsInverterStatus = "Permanent Fault Mode";
                break;
            case "105":
                QCellsInverterStatus = "Update Mode";
                break;
            case "106":
                QCellsInverterStatus = "EPS Check Mode";
                break;
            case "107":
                QCellsInverterStatus = "EPS Mode";
                break;
            case "108":
                QCellsInverterStatus = "Self - Test Mode";
                break;
            case "109":
                QCellsInverterStatus = "Idle Mode";
                break;
            case "110":
                QCellsInverterStatus = "Standby Mode";
                break;
            case "111":
                QCellsInverterStatus = "Pv Wake Up Bat Mode";
                break;
            case "112":
                QCellsInverterStatus = "Gen Check Mode";
                break;
            case "113":
                QCellsInverterStatus = "Gen Run Mode";
                break;
            default:
                QCellsInverterStatus = "Unknown status";
                break;
        }
    }
}

public class QCellDataRaw
{
    public List<int> Data { get; set; }
}
