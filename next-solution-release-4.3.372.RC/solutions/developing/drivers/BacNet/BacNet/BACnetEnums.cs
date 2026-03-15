using System;
using System.Collections;
using System.Collections.Generic;
using Opc.Ua;

namespace BACnet
{

  public class BACnetEnums
  {

    public const int MAX_OBJECT = 0x3FF;

    public const int MAX_INSTANCE = 0x3FFFFF;
    public const int INSTANCE_BITS = 22;

    public const int PROTOCOL_VERSION = 0x01;

    //public const int UNICAST_NPDU = 0x0A;

    public const byte BVLC_TYPE_BIP = 0x81;

    public const UInt32 COV_INTERVAL = 0; //Indefinite

    public const PriorityLevels DefaultPriorityLevel = PriorityLevels._8_Manual_Operator; 

    public const Int32 BACnet_DEFAULT_UDP_PORT = 0xBAC0; //47808

    public const UInt16 PROGEA_VENDOR_ID = 0;   //Ashare vendor ID

    public const UInt16 MAX_APDU_LENGTH = 480;

   // BVLC Function
   public enum BVLC_Functions
   {
        BVLC_Result = 0x00,
        WrBcastDistTable = 0x01,
        ReBcastDistTable = 0x02,
        ReBcastDistTableACK = 0x03,
        ForwardedNPDU = 0x04,
        RegisterDevice = 0x05,
        ReadDeviceTable = 0x06,
        ReadDeviceTableACK = 0x07,
        DeleteDeviceTableEntry = 0x08,
        DistBcastToNet = 0x09,
        UnicastNPDU = 0x0A,
        BcastNPDU = 0x0B,
    }
    // BVLC Result:
    public enum BVLC_Result
    {
        Successful = 0x0000,
        WrBcastDistTableNAK = 0x0010,
        ReBcastDistTableNAK = 0x0020,
        RegDeviceNAK = 0x0030,
        ReadDeviceTableNAK = 0x0040,
        DeleteDeviceTableNAK = 0x0050,
        DistBcastToNetNAK = 0x0060,
    }

    // Used Property
    public enum PropertyIdentifier
    {
        ACKED_TRANSITIONS = 0,
        PRESENT_VALUE = 85,
        STATUS_FLAGS = 111,
        MAX_PRES_VALUE = 65,
        MIN_PRES_VALUE = 69,
        ALARM_VALUES = 7,
        FEEDBACK_VALUE = 40,
        FAULT_VALUES = 39,
        HIGH_LIMIT = 45,
        LOW_LIMIT = 59,
        LIMIT_ENABLE = 52,
        OUT_OF_SERVICE = 81,
        EVENT_ENABLE = 35,
        EVENT_STATE = 36,
        NOTIFY_TYPE = 72,
        ACTIVE_TEXT = 4,
        INACTIVE_TEXT = 46,
        NUMBER_OF_STATES = 74,
        STATE_TEXT = 110,
        PRIORITY_ARRAY = 87,
        UNITS = 117,
        SCALE = 187,
        DEADBAND = 25,
        DATE_LIST = 23,
        EFFECTIVE_PERIOD = 32,
        WEEKLY_SCHEDULE = 123,
        LIST_OF_OBJECT_PROPERTY_REFERENCES = 54,
        EXCEPTION_SCHEDULE = 38,
        DESCRIPTION = 28,
        OBJECT_IDENTIFIER = 75,
        OBJECT_NAME = 77,
        OBJECT_TYPE = 79,
        SYSTEM_STATUS = 112,
        FIRMWARE_REVISION = 44,
        PROTOCOL_VERSION = 98,
        PROTOCOL_REVISION = 139,
        SCHEDULE_DEFAULT = 174,

        RELIABILITY = 103,
        MODE = 160,
        SILENCED = 163,
        TRACKING_VALUE = 164,
        MAX_APDU_LENGTH_ACCEPTED = 62,
        SEGMENTATION_SUPPORTED = 107,
        VENDOR_IDENTIFIER = 120,
        VENDOR_NAME = 121,
    }
    // Total Property
    //public enum BACnetPropertyIdentifier
    //{
    //  ACKED_TRANSITIONS = 0,
    //  ACK_REQUIRED = 1,
    //  ACTION = 2,
    //  ACTION_TEXT = 3,
    //  ACTIVE_TEXT = 4,
    //  ACTIVE_VT_SESSIONS = 5,
    //  ALARM_VALUE = 6,
    //  ALARM_VALUES = 7,
    //  ALL = 8,
    //  ALL_WRITES_SUCCESSFUL = 9,
    //  APDU_SEGMENT_TIMEOUT = 10,
    //  APDU_TIMEOUT = 11,
    //  APPLICATION_SOFTWARE_VERSION = 12,
    //  ARCHIVE = 13,
    //  BIAS = 14,
    //  CHANGE_OF_STATE_COUNT = 15,
    //  CHANGE_OF_STATE_TIME = 16,
    //  NOTIFICATION_CLASS = 17,
    //  BLANK_1 = 18,
    //  CONTROLLED_VARIABLE_REFERENCE = 19,
    //  CONTROLLED_VARIABLE_UNITS = 20,
    //  CONTROLLED_VARIABLE_VALUE = 21,
    //  COV_INCREMENT = 22,
    //  DATE_LIST = 23,
    //  DAYLIGHT_SAVINGS_STATUS = 24,
    //  DEADBAND = 25,
    //  DERIVATIVE_CONSTANT = 26,
    //  DERIVATIVE_CONSTANT_UNITS = 27,
    //  DESCRIPTION = 28,
    //  DESCRIPTION_OF_HALT = 29,
    //  DEVICE_ADDRESS_BINDING = 30,
    //  DEVICE_TYPE = 31,
    //  EFFECTIVE_PERIOD = 32,
    //  ELAPSED_ACTIVE_TIME = 33,
    //  ERROR_LIMIT = 34,
    //  EVENT_ENABLE = 35,
    //  EVENT_STATE = 36,
    //  EVENT_TYPE = 37,
    //  EXCEPTION_SCHEDULE = 38,
    //  FAULT_VALUES = 39,
    //  FEEDBACK_VALUE = 40,
    //  FILE_ACCESS_METHOD = 41,
    //  FILE_SIZE = 42,
    //  FILE_TYPE = 43,
    //  FIRMWARE_REVISION = 44,
    //  HIGH_LIMIT = 45,
    //  INACTIVE_TEXT = 46,
    //  IN_PROCESS = 47,
    //  INSTANCE_OF = 48,
    //  INTEGRAL_CONSTANT = 49,
    //  INTEGRAL_CONSTANT_UNITS = 50,
    //  ISSUE_CONFIRMED_NOTIFICATIONS = 51,
    //  LIMIT_ENABLE = 52,
    //  LIST_OF_GROUP_MEMBERS = 53,
    //  LIST_OF_OBJECT_PROPERTY_REFERENCES = 54,
    //  LIST_OF_SESSION_KEYS = 55,
    //  LOCAL_DATE = 56,
    //  LOCAL_TIME = 57,
    //  LOCATION = 58,
    //  LOW_LIMIT = 59,
    //  MANIPULATED_VARIABLE_REFERENCE = 60,
    //  MAXIMUM_OUTPUT = 61,
    //  MAX_APDU_LENGTH_ACCEPTED = 62,
    //  MAX_INFO_FRAMES = 63,
    //  MAX_MASTER = 64,
    //  MAX_PRES_VALUE = 65,
    //  MINIMUM_OFF_TIME = 66,
    //  MINIMUM_ON_TIME = 67,
    //  MINIMUM_OUTPUT = 68,
    //  MIN_PRES_VALUE = 69,
    //  MODEL_NAME = 70,
    //  MODIFICATION_DATE = 71,
    //  NOTIFY_TYPE = 72,
    //  NUMBER_OF_APDU_RETRIES = 73,
    //  NUMBER_OF_STATES = 74,
    //  OBJECT_IDENTIFIER = 75,
    //  OBJECT_LIST = 76,
    //  OBJECT_NAME = 77,
    //  OBJECT_PROPERTY_REFERENCE = 78,
    //  OBJECT_TYPE = 79,
    //  OPTIONAL = 80,
    //  OUT_OF_SERVICE = 81,
    //  OUTPUT_UNITS = 82,
    //  EVENT_PARAMETERS = 83,
    //  POLARITY = 84,
    //  PRESENT_VALUE = 85,
    //  PRIORITY = 86,
    //  PRIORITY_ARRAY = 87,
    //  PRIORITY_FOR_WRITING = 88,
    //  PROCESS_IDENTIFIER = 89,
    //  PROGRAM_CHANGE = 90,
    //  PROGRAM_LOCATION = 91,
    //  PROGRAM_STATE = 92,
    //  PROPORTIONAL_CONSTANT = 93,
    //  PROPORTIONAL_CONSTANT_UNITS = 94,
    //  PROTOCOL_CONFORMANCE_CLASS = 95,       /* deleted in version 1 revision 2 */
    //  PROTOCOL_OBJECT_TYPES_SUPPORTED = 96,
    //  PROTOCOL_SERVICES_SUPPORTED = 97,
    //  PROTOCOL_VERSION = 98,
    //  READ_ONLY = 99,
    //  REASON_FOR_HALT = 100,
    //  RECIPIENT = 101,
    //  RECIPIENT_LIST = 102,
    //  RELIABILITY = 103,
    //  RELINQUISH_DEFAULT = 104,
    //  REQUIRED = 105,
    //  RESOLUTION = 106,
    //  SEGMENTATION_SUPPORTED = 107,
    //  SETPOINT = 108,
    //  SETPOINT_REFERENCE = 109,
    //  STATE_TEXT = 110,
    //  STATUS_FLAGS = 111,
    //  SYSTEM_STATUS = 112,
    //  TIME_DELAY = 113,
    //  TIME_OF_ACTIVE_TIME_RESET = 114,
    //  TIME_OF_STATE_COUNT_RESET = 115,
    //  TIME_SYNCHRONIZATION_RECIPIENTS = 116,
    //  UNITS = 117,
    //  UPDATE_INTERVAL = 118,
    //  UTC_OFFSET = 119,
    //  VENDOR_IDENTIFIER = 120,
    //  VENDOR_NAME = 121,
    //  VT_CLASSES_SUPPORTED = 122,
    //  WEEKLY_SCHEDULE = 123,
    //  ATTEMPTED_SAMPLES = 124,
    //  AVERAGE_VALUE = 125,
    //  BUFFER_SIZE = 126,
    //  CLIENT_COV_INCREMENT = 127,
    //  COV_RESUBSCRIPTION_INTERVAL = 128,
    //  CURRENT_NOTIFY_TIME = 129,
    //  EVENT_TIME_STAMPS = 130,
    //  LOG_BUFFER = 131,
    //  LOG_DEVICE_OBJECT = 132,
    //  /* The enable property is renamed from log-enable in
    //     Addendum b to ANSI/ASHRAE 135-2004(135b-2) */
    //  ENABLE = 133,
    //  LOG_INTERVAL = 134,
    //  MAXIMUM_VALUE = 135,
    //  MINIMUM_VALUE = 136,
    //  NOTIFICATION_THRESHOLD = 137,
    //  PREVIOUS_NOTIFY_TIME = 138,
    //  PROTOCOL_REVISION = 139,
    //  RECORDS_SINCE_NOTIFICATION = 140,
    //  RECORD_COUNT = 141,
    //  START_TIME = 142,
    //  STOP_TIME = 143,
    //  STOP_WHEN_FULL = 144,
    //  TOTAL_RECORD_COUNT = 145,
    //  VALID_SAMPLES = 146,
    //  WINDOW_INTERVAL = 147,
    //  WINDOW_SAMPLES = 148,
    //  MAXIMUM_VALUE_TIMESTAMP = 149,
    //  MINIMUM_VALUE_TIMESTAMP = 150,
    //  VARIANCE_VALUE = 151,
    //  ACTIVE_COV_SUBSCRIPTIONS = 152,
    //  BACKUP_FAILURE_TIMEOUT = 153,
    //  CONFIGURATION_FILES = 154,
    //  DATABASE_REVISION = 155,
    //  DIRECT_READING = 156,
    //  LAST_RESTORE_TIME = 157,
    //  MAINTENANCE_REQUIRED = 158,
    //  MEMBER_OF = 159,
    //  MODE = 160,
    //  OPERATION_EXPECTED = 161,
    //  SETTING = 162,
    //  SILENCED = 163,
    //  TRACKING_VALUE = 164,
    //  ZONE_MEMBERS = 165,
    //  LIFE_SAFETY_ALARM_VALUES = 166,
    //  MAX_SEGMENTS_ACCEPTED = 167,
    //  PROFILE_NAME = 168,
    //  AUTO_SLAVE_DISCOVERY = 169,
    //  MANUAL_SLAVE_ADDRESS_BINDING = 170,
    //  SLAVE_ADDRESS_BINDING = 171,
    //  SLAVE_PROXY_ENABLE = 172,
    //  LAST_NOTIFY_TIME = 173,
    //  SCHEDULE_DEFAULT = 174,
    //  ACCEPTED_MODES = 175,
    //  ADJUST_VALUE = 176,
    //  COUNT = 177,
    //  COUNT_BEFORE_CHANGE = 178,
    //  COUNT_CHANGE_TIME = 179,
    //  COV_PERIOD = 180,
    //  INPUT_REFERENCE = 181,
    //  LIMIT_MONITORING_INTERVAL = 182,
    //  LOGGING_DEVICE = 183,
    //  LOGGING_RECORD = 184,
    //  PRESCALE = 185,
    //  PULSE_RATE = 186,
    //  SCALE = 187,
    //  SCALE_FACTOR = 188,
    //  UPDATE_TIME = 189,
    //  VALUE_BEFORE_CHANGE = 190,
    //  VALUE_SET = 191,
    //  VALUE_CHANGE_TIME = 192,
    //  /* enumerations 193-206 are new */
    //  ALIGN_INTERVALS = 193,
    //  GROUP_MEMBER_NAMES = 194,
    //  INTERVAL_OFFSET = 195,
    //  LAST_RESTART_REASON = 196,
    //  LOGGING_TYPE = 197,
    //  MEMBER_STATUS_FLAGS = 198,
    //  NOTIFICATION_PERIOD = 199,
    //  PREVIOUS_NOTIFY_RECORD = 200,
    //  REQUESTED_UPDATE_INTERVAL = 201,
    //  RESTART_NOTIFICATION_RECIPIENTS = 202,
    //  TIME_OF_DEVICE_RESTART = 203,
    //  TIME_SYNCHRONIZATION_INTERVAL = 204,
    //  TRIGGER = 205,
    //  UTC_TIME_SYNCHRONIZATION_RECIPIENTS = 206,
    //  /* enumerations 207-211 are used in Addendum d to ANSI/ASHRAE 135-2004 */
    //  NODE_SUBTYPE = 207,
    //  NODE_TYPE = 208,
    //  STRUCTURED_OBJECT_LIST = 209,
    //  SUBORDINATE_ANNOTATIONS = 210,
    //  SUBORDINATE_LIST = 211,
    //  /* enumerations 212-225 are used in Addendum e to ANSI/ASHRAE 135-2004 */
    //  ACTUAL_SHED_LEVEL = 212,
    //  DUTY_WINDOW = 213,
    //  EXPECTED_SHED_LEVEL = 214,
    //  FULL_DUTY_BASELINE = 215,
    //  /* enumerations 216-217 are used in Addendum i to ANSI/ASHRAE 135-2004 */
    //  BLINK_PRIORITY_THRESHOLD = 216,
    //  BLINK_TIME = 217,
    //  /* enumerations 212-225 are used in Addendum e to ANSI/ASHRAE 135-2004 */
    //  REQUESTED_SHED_LEVEL = 218,
    //  SHED_DURATION = 219,
    //  SHED_LEVEL_DESCRIPTIONS = 220,
    //  SHED_LEVELS = 221,
    //  STATE_DESCRIPTION = 222,
    //  /* enumerations 223-225 are used in Addendum i to ANSI/ASHRAE 135-2004 */
    //  FADE_TIME = 223,
    //  LIGHTING_COMMAND = 224,
    //  LIGHTING_COMMAND_PRIORITY = 225,
    //  /* enumerations 226-235 are used in Addendum f to ANSI/ASHRAE 135-2004 */
    //  /* enumerations 236-243 are used in Addendum i to ANSI/ASHRAE 135-2004 */
    //  OFF_DELAY = 236,
    //  ON_DELAY = 237,
    //  POWER = 238,
    //  POWER_ON_VALUE = 239,
    //  PROGRESS_VALUE = 240,
    //  RAMP_RATE = 241,
    //  STEP_INCREMENT = 242,
    //  SYSTEM_FAILURE_VALUE = 243,
    //  /* The special property identifiers all, optional, and required  */
    //  /* are reserved for use in the ReadPropertyConditional and */
    //  /* ReadPropertyMultiple services or services not defined in this standard. */
    //  /* Enumerated values 0-511 are reserved for definition by ASHRAE.  */
    //  /* Enumerated values 512-4194303 may be used by others subject to the  */
    //  /* procedures and constraints described in Clause 23.  */
    //  BAUD_RATE = 9600,
    //  SERIAL_NUMBER = 9701
    //}
    public const UInt16 MAX_BACnet_PROPERTY_ID = 0x7fff;
    public const UInt16 DEFAULT_ARRAY_ID = 0;
    public const UInt32 DEFAULT_DEVICE_ID = 0;
    public const Int32 TimeSize = 4;
    public const Int32 TimeValueValue = 5;
    public const Int32 TimeValueSize = 13;
    public const Int32 CalendarEntrySize = 9;
    public const Int32 PropertyIdentifierSize = 12;

    public enum prototipe
    {
        noPrototipe,
        Date,
        Event,
        DeviceObjectPropertyReference,
        DataRange,
        ScheduleSpecialEvent,
        ListOfObjectPropertyReference,
        ScheduleDay,
        DateList,
        CalendarEntry,
    }
    public enum BACnet_ACTION
    {
      ACTION_DIRECT = 0,
      ACTION_REVERSE = 1
    }

    public enum BACnet_BINARY_PV
    {
      MIN_BINARY_PV = 0,  /* for validating incoming values */
      BINARY_INACTIVE = 0,
      BINARY_ACTIVE = 1,
      MAX_BINARY_PV = 1,  /* for validating incoming values */
      BINARY_NULL = 2     /* our homemade way of storing this info */
    }

    public enum BACnet_ACTION_VALUE_TYPE
    {
      ACTION_BINARY_PV,
      ACTION_UNSIGNED,
      ACTION_FLOAT
    }

    public enum BACnet_EVENT_STATE
    {
      EVENT_STATE_NORMAL = 0,
      EVENT_STATE_FAULT = 1,
      EVENT_STATE_OFFNORMAL = 2,
      EVENT_STATE_HIGH_LIMIT = 3,
      EVENT_STATE_LOW_LIMIT = 4,
      EVENT_STATE_LIFE_SAFETY_ALARM = 5
    }

    public enum BACnet_DEVICE_STATUS
    {
      STATUS_OPERATIONAL = 0,
      STATUS_OPERATIONAL_READ_ONLY = 1,
      STATUS_DOWNLOAD_REQUIRED = 2,
      STATUS_DOWNLOAD_IN_PROGRESS = 3,
      STATUS_NON_OPERATIONAL = 4,
      MAX_DEVICE_STATUS = 5
    }

    public enum BACnet_ENGINEERING_UNITS
    {
      /* Acceleration */
      UNITS_METERS_PER_SECOND_PER_SECOND = 166,
      /* Area */
      UNITS_SQUARE_METERS = 0,
      UNITS_SQUARE_CENTIMETERS = 116,
      UNITS_SQUARE_FEET = 1,
      UNITS_SQUARE_INCHES = 115,
      /* Currency */
      UNITS_CURRENCY1 = 105,
      UNITS_CURRENCY2 = 106,
      UNITS_CURRENCY3 = 107,
      UNITS_CURRENCY4 = 108,
      UNITS_CURRENCY5 = 109,
      UNITS_CURRENCY6 = 110,
      UNITS_CURRENCY7 = 111,
      UNITS_CURRENCY8 = 112,
      UNITS_CURRENCY9 = 113,
      UNITS_CURRENCY10 = 114,
      /* Electrical */
      UNITS_MILLIAMPERES = 2,
      UNITS_AMPERES = 3,
      UNITS_AMPERES_PER_METER = 167,
      UNITS_AMPERES_PER_SQUARE_METER = 168,
      UNITS_AMPERE_SQUARE_METERS = 169,
      UNITS_FARADS = 170,
      UNITS_HENRYS = 171,
      UNITS_OHMS = 4,
      UNITS_OHM_METERS = 172,
      UNITS_MILLIOHMS = 145,
      UNITS_KILOHMS = 122,
      UNITS_MEGOHMS = 123,
      UNITS_SIEMENS = 173,        /* 1 mho equals 1 siemens */
      UNITS_SIEMENS_PER_METER = 174,
      UNITS_TESLAS = 175,
      UNITS_VOLTS = 5,
      UNITS_MILLIVOLTS = 124,
      UNITS_KILOVOLTS = 6,
      UNITS_MEGAVOLTS = 7,
      UNITS_VOLT_AMPERES = 8,
      UNITS_KILOVOLT_AMPERES = 9,
      UNITS_MEGAVOLT_AMPERES = 10,
      UNITS_VOLT_AMPERES_REACTIVE = 11,
      UNITS_KILOVOLT_AMPERES_REACTIVE = 12,
      UNITS_MEGAVOLT_AMPERES_REACTIVE = 13,
      UNITS_VOLTS_PER_DEGREE_KELVIN = 176,
      UNITS_VOLTS_PER_METER = 177,
      UNITS_DEGREES_PHASE = 14,
      UNITS_POWER_FACTOR = 15,
      UNITS_WEBERS = 178,
      /* Energy */
      UNITS_JOULES = 16,
      UNITS_KILOJOULES = 17,
      UNITS_KILOJOULES_PER_KILOGRAM = 125,
      UNITS_MEGAJOULES = 126,
      UNITS_WATT_HOURS = 18,
      UNITS_KILOWATT_HOURS = 19,
      UNITS_MEGAWATT_HOURS = 146,
      UNITS_BTUS = 20,
      UNITS_KILO_BTUS = 147,
      UNITS_MEGA_BTUS = 148,
      UNITS_THERMS = 21,
      UNITS_TON_HOURS = 22,
      /* Enthalpy */
      UNITS_JOULES_PER_KILOGRAM_DRY_AIR = 23,
      UNITS_KILOJOULES_PER_KILOGRAM_DRY_AIR = 149,
      UNITS_MEGAJOULES_PER_KILOGRAM_DRY_AIR = 150,
      UNITS_BTUS_PER_POUND_DRY_AIR = 24,
      UNITS_BTUS_PER_POUND = 117,
      /* Entropy */
      UNITS_JOULES_PER_DEGREE_KELVIN = 127,
      UNITS_KILOJOULES_PER_DEGREE_KELVIN = 151,
      UNITS_MEGAJOULES_PER_DEGREE_KELVIN = 152,
      UNITS_JOULES_PER_KILOGRAM_DEGREE_KELVIN = 128,
      /* Force */
      UNITS_NEWTON = 153,
      /* Frequency */
      UNITS_CYCLES_PER_HOUR = 25,
      UNITS_CYCLES_PER_MINUTE = 26,
      UNITS_HERTZ = 27,
      UNITS_KILOHERTZ = 129,
      UNITS_MEGAHERTZ = 130,
      UNITS_PER_HOUR = 131,
      /* Humidity */
      UNITS_GRAMS_OF_WATER_PER_KILOGRAM_DRY_AIR = 28,
      UNITS_PERCENT_RELATIVE_HUMIDITY = 29,
      /* Length */
      UNITS_MILLIMETERS = 30,
      UNITS_CENTIMETERS = 118,
      UNITS_METERS = 31,
      UNITS_INCHES = 32,
      UNITS_FEET = 33,
      /* Light */
      UNITS_CANDELAS = 179,
      UNITS_CANDELAS_PER_SQUARE_METER = 180,
      UNITS_WATTS_PER_SQUARE_FOOT = 34,
      UNITS_WATTS_PER_SQUARE_METER = 35,
      UNITS_LUMENS = 36,
      UNITS_LUXES = 37,
      UNITS_FOOT_CANDLES = 38,
      /* Mass */
      UNITS_KILOGRAMS = 39,
      UNITS_POUNDS_MASS = 40,
      UNITS_TONS = 41,
      /* Mass Flow */
      UNITS_GRAMS_PER_SECOND = 154,
      UNITS_GRAMS_PER_MINUTE = 155,
      UNITS_KILOGRAMS_PER_SECOND = 42,
      UNITS_KILOGRAMS_PER_MINUTE = 43,
      UNITS_KILOGRAMS_PER_HOUR = 44,
      UNITS_POUNDS_MASS_PER_SECOND = 119,
      UNITS_POUNDS_MASS_PER_MINUTE = 45,
      UNITS_POUNDS_MASS_PER_HOUR = 46,
      UNITS_TONS_PER_HOUR = 156,
      /* Power */
      UNITS_MILLIWATTS = 132,
      UNITS_WATTS = 47,
      UNITS_KILOWATTS = 48,
      UNITS_MEGAWATTS = 49,
      UNITS_BTUS_PER_HOUR = 50,
      UNITS_KILO_BTUS_PER_HOUR = 157,
      UNITS_HORSEPOWER = 51,
      UNITS_TONS_REFRIGERATION = 52,
      /* Pressure */
      UNITS_PASCALS = 53,
      UNITS_HECTOPASCALS = 133,
      UNITS_KILOPASCALS = 54,
      UNITS_MILLIBARS = 134,
      UNITS_BARS = 55,
      UNITS_POUNDS_FORCE_PER_SQUARE_INCH = 56,
      UNITS_CENTIMETERS_OF_WATER = 57,
      UNITS_INCHES_OF_WATER = 58,
      UNITS_MILLIMETERS_OF_MERCURY = 59,
      UNITS_CENTIMETERS_OF_MERCURY = 60,
      UNITS_INCHES_OF_MERCURY = 61,
      /* Temperature */
      UNITS_DEGREES_CELSIUS = 62,
      UNITS_DEGREES_KELVIN = 63,
      UNITS_DEGREES_KELVIN_PER_HOUR = 181,
      UNITS_DEGREES_KELVIN_PER_MINUTE = 182,
      UNITS_DEGREES_FAHRENHEIT = 64,
      UNITS_DEGREE_DAYS_CELSIUS = 65,
      UNITS_DEGREE_DAYS_FAHRENHEIT = 66,
      UNITS_DELTA_DEGREES_FAHRENHEIT = 120,
      UNITS_DELTA_DEGREES_KELVIN = 121,
      /* Time */
      UNITS_YEARS = 67,
      UNITS_MONTHS = 68,
      UNITS_WEEKS = 69,
      UNITS_DAYS = 70,
      UNITS_HOURS = 71,
      UNITS_MINUTES = 72,
      UNITS_SECONDS = 73,
      UNITS_HUNDREDTHS_SECONDS = 158,
      UNITS_MILLISECONDS = 159,
      /* Torque */
      UNITS_NEWTON_METERS = 160,
      /* Velocity */
      UNITS_MILLIMETERS_PER_SECOND = 161,
      UNITS_MILLIMETERS_PER_MINUTE = 162,
      UNITS_METERS_PER_SECOND = 74,
      UNITS_METERS_PER_MINUTE = 163,
      UNITS_METERS_PER_HOUR = 164,
      UNITS_KILOMETERS_PER_HOUR = 75,
      UNITS_FEET_PER_SECOND = 76,
      UNITS_FEET_PER_MINUTE = 77,
      UNITS_MILES_PER_HOUR = 78,
      /* Volume */
      UNITS_CUBIC_FEET = 79,
      UNITS_CUBIC_METERS = 80,
      UNITS_IMPERIAL_GALLONS = 81,
      UNITS_LITERS = 82,
      UNITS_US_GALLONS = 83,
      /* Volumetric Flow */
      UNITS_CUBIC_FEET_PER_SECOND = 142,
      UNITS_CUBIC_FEET_PER_MINUTE = 84,
      UNITS_CUBIC_METERS_PER_SECOND = 85,
      UNITS_CUBIC_METERS_PER_MINUTE = 165,
      UNITS_CUBIC_METERS_PER_HOUR = 135,
      UNITS_IMPERIAL_GALLONS_PER_MINUTE = 86,
      UNITS_LITERS_PER_SECOND = 87,
      UNITS_LITERS_PER_MINUTE = 88,
      UNITS_LITERS_PER_HOUR = 136,
      UNITS_US_GALLONS_PER_MINUTE = 89,
      /* Other */
      UNITS_DEGREES_ANGULAR = 90,
      UNITS_DEGREES_CELSIUS_PER_HOUR = 91,
      UNITS_DEGREES_CELSIUS_PER_MINUTE = 92,
      UNITS_DEGREES_FAHRENHEIT_PER_HOUR = 93,
      UNITS_DEGREES_FAHRENHEIT_PER_MINUTE = 94,
      UNITS_JOULE_SECONDS = 183,
      UNITS_KILOGRAMS_PER_CUBIC_METER = 186,
      UNITS_KW_HOURS_PER_SQUARE_METER = 137,
      UNITS_KW_HOURS_PER_SQUARE_FOOT = 138,
      UNITS_MEGAJOULES_PER_SQUARE_METER = 139,
      UNITS_MEGAJOULES_PER_SQUARE_FOOT = 140,
      UNITS_NO_UNITS = 95,
      UNITS_NEWTON_SECONDS = 187,
      UNITS_NEWTONS_PER_METER = 188,
      UNITS_PARTS_PER_MILLION = 96,
      UNITS_PARTS_PER_BILLION = 97,
      UNITS_PERCENT = 98,
      UNITS_PERCENT_OBSCURATION_PER_FOOT = 143,
      UNITS_PERCENT_OBSCURATION_PER_METER = 144,
      UNITS_PERCENT_PER_SECOND = 99,
      UNITS_PER_MINUTE = 100,
      UNITS_PER_SECOND = 101,
      UNITS_PSI_PER_DEGREE_FAHRENHEIT = 102,
      UNITS_RADIANS = 103,
      UNITS_RADIANS_PER_SECOND = 184,
      UNITS_REVOLUTIONS_PER_MINUTE = 104,
      UNITS_SQUARE_METERS_PER_NEWTON = 185,
      UNITS_WATTS_PER_METER_PER_DEGREE_KELVIN = 189,
      UNITS_WATTS_PER_SQUARE_METER_DEGREE_KELVIN = 141
      /* Enumerated values 0-255 are reserved for definition by ASHRAE. */
      /* Enumerated values 256-65535 may be used by others subject to */
      /* the procedures and constraints described in Clause 23. */
      /* The last enumeration used in this version is 189. */
    }

    public enum BACnet_POLARITY
    {
      POLARITY_NORMAL = 0,
      POLARITY_REVERSE = 1
    }

    public enum BACnet_PROGRAM_REQUEST
    {
      PROGRAM_REQUEST_READY = 0,
      PROGRAM_REQUEST_LOAD = 1,
      PROGRAM_REQUEST_RUN = 2,
      PROGRAM_REQUEST_HALT = 3,
      PROGRAM_REQUEST_RESTART = 4,
      PROGRAM_REQUEST_UNLOAD = 5
    }

    public enum BACnet_PROGRAM_STATE
    {
      PROGRAM_STATE_IDLE = 0,
      PROGRAM_STATE_LOADING = 1,
      PROGRAM_STATE_RUNNING = 2,
      PROGRAM_STATE_WAITING = 3,
      PROGRAM_STATE_HALTED = 4,
      PROGRAM_STATE_UNLOADING = 5
    }

    public enum BACnet_PROGRAM_ERROR
    {
      PROGRAM_ERROR_NORMAL = 0,
      PROGRAM_ERROR_LOAD_FAILED = 1,
      PROGRAM_ERROR_INTERNAL = 2,
      PROGRAM_ERROR_PROGRAM = 3,
      PROGRAM_ERROR_OTHER = 4
      /* Enumerated values 0-63 are reserved for definition by ASHRAE.  */
      /* Enumerated values 64-65535 may be used by others subject to  */
      /* the procedures and constraints described in Clause 23. */
    }

    public enum BACnet_RELIABILITY
    {
      RELIABILITY_NO_FAULT_DETECTED = 0,
      RELIABILITY_NO_SENSOR = 1,
      RELIABILITY_OVER_RANGE = 2,
      RELIABILITY_UNDER_RANGE = 3,
      RELIABILITY_OPEN_LOOP = 4,
      RELIABILITY_SHORTED_LOOP = 5,
      RELIABILITY_NO_OUTPUT = 6,
      RELIABILITY_UNRELIABLE_OTHER = 7,
      RELIABILITY_PROCESS_ERROR = 8,
      RELIABILITY_MULTI_STATE_FAULT = 9,
      RELIABILITY_CONFIGURATION_ERROR = 10,
      RELIABILITY_COMMUNICATION_FAILURE = 12,
      RELIABILITY_TRIPPED = 13
      /* Enumerated values 0-63 are reserved for definition by ASHRAE.  */
      /* Enumerated values 64-65535 may be used by others subject to  */
      /* the procedures and constraints described in Clause 23. */
    }

    public enum BACnet_EVENT_TYPE
    {
      EVENT_CHANGE_OF_BITSTRING = 0,
      EVENT_CHANGE_OF_STATE = 1,
      EVENT_CHANGE_OF_VALUE = 2,
      EVENT_COMMAND_FAILURE = 3,
      EVENT_FLOATING_LIMIT = 4,
      EVENT_OUT_OF_RANGE = 5,
      /*  complex-event-type        (6), -- see comment below */
      /*  event-buffer-ready   (7), -- context tag 7 is deprecated */
      EVENT_CHANGE_OF_LIFE_SAFETY = 8,
      EVENT_EXTENDED = 9,
      EVENT_BUFFER_READY = 10,
      EVENT_UNSIGNED_RANGE = 11
      /* Enumerated values 0-63 are reserved for definition by ASHRAE.  */
      /* Enumerated values 64-65535 may be used by others subject to  */
      /* the procedures and constraints described in Clause 23.  */
      /* It is expected that these enumerated values will correspond to  */
      /* the use of the complex-event-type CHOICE [6] of the  */
      /* BACnetNotificationParameters production. */
      /* The last enumeration used in this version is 11. */
    }

    public enum BACnet_FILE_ACCESS_METHOD
    {
      FILE_RECORD_ACCESS = 0,
      FILE_STREAM_ACCESS = 1,
      FILE_RECORD_AND_STREAM_ACCESS = 2
    }

    public enum BACnet_LIFE_SAFETY_MODE
    {
      MIN_LIFE_SAFETY_MODE = 0,
      LIFE_SAFETY_MODE_OFF = 0,
      LIFE_SAFETY_MODE_ON = 1,
      LIFE_SAFETY_MODE_TEST = 2,
      LIFE_SAFETY_MODE_MANNED = 3,
      LIFE_SAFETY_MODE_UNMANNED = 4,
      LIFE_SAFETY_MODE_ARMED = 5,
      LIFE_SAFETY_MODE_DISARMED = 6,
      LIFE_SAFETY_MODE_PREARMED = 7,
      LIFE_SAFETY_MODE_SLOW = 8,
      LIFE_SAFETY_MODE_FAST = 9,
      LIFE_SAFETY_MODE_DISCONNECTED = 10,
      LIFE_SAFETY_MODE_ENABLED = 11,
      LIFE_SAFETY_MODE_DISABLED = 12,
      LIFE_SAFETY_MODE_AUTOMATIC_RELEASE_DISABLED = 13,
      LIFE_SAFETY_MODE_DEFAULT = 14,
      MAX_LIFE_SAFETY_MODE = 15,
        /* Enumerated values 0-255 are reserved for definition by ASHRAE.  */
        /* Enumerated values 256-65535 may be used by others subject to  */
        /* procedures and constraints described in Clause 23. */
        /* do the max range inside of enum so that
            compilers will allocate adequate sized datatype for enum
            which is used to store decoding */
      LIFE_SAFETY_MODE_PROPRIETARY_MIN = 256,
      LIFE_SAFETY_MODE_PROPRIETARY_MAX = 65535
    }

    public enum BACnet_LIFE_SAFETY_OPERATION
    {
      LIFE_SAFETY_OP_NONE = 0,
      LIFE_SAFETY_OP_SILENCE = 1,
      LIFE_SAFETY_OP_SILENCE_AUDIBLE = 2,
      LIFE_SAFETY_OP_SILENCE_VISUAL = 3,
      LIFE_SAFETY_OP_RESET = 4,
      LIFE_SAFETY_OP_RESET_ALARM = 5,
      LIFE_SAFETY_OP_RESET_FAULT = 6,
      LIFE_SAFETY_OP_UNSILENCE = 7,
      LIFE_SAFETY_OP_UNSILENCE_AUDIBLE = 8,
      LIFE_SAFETY_OP_UNSILENCE_VISUAL = 9,
        /* Enumerated values 0-63 are reserved for definition by ASHRAE.  */
        /* Enumerated values 64-65535 may be used by others subject to  */
        /* procedures and constraints described in Clause 23. */
        /* do the max range inside of enum so that
            compilers will allocate adequate sized datatype for enum
            which is used to store decoding */
        LIFE_SAFETY_OP_PROPRIETARY_MIN = 64,
        LIFE_SAFETY_OP_PROPRIETARY_MAX = 65535
     }

    public enum BACnet_LIFE_SAFETY_STATE
    {
      MIN_LIFE_SAFETY_STATE = 0,
      LIFE_SAFETY_STATE_QUIET = 0,
      LIFE_SAFETY_STATE_PRE_ALARM = 1,
      LIFE_SAFETY_STATE_ALARM = 2,
      LIFE_SAFETY_STATE_FAULT = 3,
      LIFE_SAFETY_STATE_FAULT_PRE_ALARM = 4,
      LIFE_SAFETY_STATE_FAULT_ALARM = 5,
      LIFE_SAFETY_STATE_NOT_READY = 6,
      LIFE_SAFETY_STATE_ACTIVE = 7,
      LIFE_SAFETY_STATE_TAMPER = 8,
      LIFE_SAFETY_STATE_TEST_ALARM = 9,
      LIFE_SAFETY_STATE_TEST_ACTIVE = 10,
      LIFE_SAFETY_STATE_TEST_FAULT = 11,
      LIFE_SAFETY_STATE_TEST_FAULT_ALARM = 12,
      LIFE_SAFETY_STATE_HOLDUP = 13,
      LIFE_SAFETY_STATE_DURESS = 14,
      LIFE_SAFETY_STATE_TAMPER_ALARM = 15,
      LIFE_SAFETY_STATE_ABNORMAL = 16,
      LIFE_SAFETY_STATE_EMERGENCY_POWER = 17,
      LIFE_SAFETY_STATE_DELAYED = 18,
      LIFE_SAFETY_STATE_BLOCKED = 19,
      LIFE_SAFETY_STATE_LOCAL_ALARM = 20,
      LIFE_SAFETY_STATE_GENERAL_ALARM = 21,
      LIFE_SAFETY_STATE_SUPERVISORY = 22,
      LIFE_SAFETY_STATE_TEST_SUPERVISORY = 23,
      MAX_LIFE_SAFETY_STATE = 24,
        /* Enumerated values 0-255 are reserved for definition by ASHRAE.  */
        /* Enumerated values 256-65535 may be used by others subject to  */
        /* procedures and constraints described in Clause 23. */
        /* do the max range inside of enum so that
            compilers will allocate adequate sized datatype for enum
            which is used to store decoding */
      LIFE_SAFETY_STATE_PROPRIETARY_MIN = 256,
      LIFE_SAFETY_STATE_PROPRIETARY_MAX = 65535
    }

    public enum BACnet_SILENCED_STATE
    {
      SILENCED_STATE_UNSILENCED = 0,
      SILENCED_STATE_AUDIBLE_SILENCED = 1,
      SILENCED_STATE_VISIBLE_SILENCED = 2,
      SILENCED_STATE_ALL_SILENCED = 3
      /* Enumerated values 0-63 are reserved for definition by ASHRAE. */
      /* Enumerated values 64-65535 may be used by others subject to */
      /* procedures and constraints described in Clause 23. */
    }

    public enum BACnet_MAINTENANCE
    {
      MAINTENANCE_NONE = 0,
      MAINTENANCE_PERIODIC_TEST = 1,
      AINTENANCE_NEED_SERVICE_OPERATIONAL = 2,
      MAINTENANCE_NEED_SERVICE_INOPERATIVE = 3
      /* Enumerated values 0-255 are reserved for definition by ASHRAE.  */
      /* Enumerated values 256-65535 may be used by others subject to  */
      /* procedures and constraints described in Clause 23. */
    }

    public enum BACnet_NOTIFY_TYPE
    {
      NOTIFY_ALARM = 0,
      NOTIFY_EVENT = 1,
      NOTIFY_ACK_NOTIFICATION = 2
    }

    // Used Object
    public enum ObjectTypes
    {
      DEVICE = 8,
      ANALOG_INPUT = 0,
      ANALOG_OUTPUT = 1,
      ANALOG_VALUE = 2,
      BINARY_INPUT = 3,
      BINARY_OUTPUT = 4,
      BINARY_VALUE = 5,
      MULTI_STATE_INPUT = 13,
      MULTI_STATE_OUTPUT = 14,
      MULTI_STATE_VALUE = 19,
      LIFE_SAFETY_POINT =21,
      LIFE_SAFETY_ZONE =22,
      ACCUMULATOR = 23,
      CALENDAR = 6,
      SCHEDULE = 17,
      OBJECT_NULL = -1,
   }
        // Total Object
        // public enum BACnetObjectType
        //{
        //    //OBJECT_NULL = -1,
        //    ANALOG_INPUT = 0,
        //    ANALOG_OUTPUT = 1,
        //    ANALOG_VALUE = 2,
        //    BINARY_INPUT = 3,
        //    BINARY_OUTPUT = 4,
        //    BINARY_VALUE = 5,
        //    CALENDAR = 6,
        //    COMMAND = 7,
        //    DEVICE = 8,
        //    EVENT_ENROLLMENT = 9,
        //    FILE = 10,
        //    GROUP = 11,
        //    LOOP = 12,
        //    MULTI_STATE_INPUT = 13,
        //    MULTI_STATE_OUTPUT = 14,
        //    NOTIFICATION_CLASS = 15,
        //    PROGRAM = 16,
        //    SCHEDULE = 17,
        //    AVERAGING = 18,
        //    MULTI_STATE_VALUE = 19,
        //    TRENDLOG = 20,
        //    LIFE_SAFETY_POINT = 21,
        //    LIFE_SAFETY_ZONE = 22,
        //    ACCUMULATOR = 23,
        //    PULSE_CONVERTER = 24,
        //    EVENT_LOG = 25,
        //    GLOBAL_GROUP = 26,
        //    TREND_LOG_MULTIPLE = 27,
        //    LOAD_CONTROL = 28,
        //    STRUCTURED_VIEW = 29,
        //    /* what is object type 30? */
        //    LIGHTING_OUTPUT = 31,
        //    /* Enumerated values 0-127 are reserved for definition by ASHRAE. */
        //    /* Enumerated values 128-1023 may be used by others subject to  */
        //    /* the procedures and constraints described in Clause 23. */
        //    MAX_ASHRAE_OBJECT_TYPE = 32,        /* used for bit string loop */
        //    MAX_BACnet_OBJECT_TYPE = 1023
        //}

        public static bool isCovSupported(string BACnetObjectType, string PropertyIdentifier)
    {
        return ((PropertyIdentifier == "PRESENT_VALUE" || PropertyIdentifier == "STATUS_FLAGS") &&
            BACnetObjectType != "DEVICE" && BACnetObjectType != "CALENDAR" &&
            BACnetObjectType != "ACCUMULATOR" && BACnetObjectType != "SCHEDULE");


    }
        public static bool isPrioritySupported(string BACnetObjectType, string PropertyIdentifier)
        {
            return (PropertyIdentifier == "PRESENT_VALUE"  &&(
                BACnetObjectType == "ANALOG_OUTPUT" || BACnetObjectType == "ANALOG_VALUE" ||
                BACnetObjectType == "BINARY_OUTPUT" || BACnetObjectType == "BINARY_VALUE" ||
                BACnetObjectType == "MULTI_STATE_OUTPUT" || BACnetObjectType == "MULTI_STATE_VALUE"));
        }

        public static readonly Dictionary<string, List<PropertyIdentifier>> ObjectPropertyDictionaryAll
        = new Dictionary<string, List<PropertyIdentifier>>
            {
                { ObjectTypes.ANALOG_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.MIN_PRES_VALUE,
                        PropertyIdentifier.MAX_PRES_VALUE,
                        PropertyIdentifier.HIGH_LIMIT,
                        PropertyIdentifier.LOW_LIMIT,
                        PropertyIdentifier.DEADBAND ,
                        PropertyIdentifier.LIMIT_ENABLE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.ANALOG_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.MIN_PRES_VALUE,
                        PropertyIdentifier.MAX_PRES_VALUE,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.HIGH_LIMIT,
                        PropertyIdentifier.LOW_LIMIT,
                        PropertyIdentifier.DEADBAND ,
                        PropertyIdentifier.LIMIT_ENABLE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.ANALOG_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.HIGH_LIMIT,
                        PropertyIdentifier.LOW_LIMIT,
                        PropertyIdentifier.DEADBAND ,
                        PropertyIdentifier.LIMIT_ENABLE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.BINARY_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.INACTIVE_TEXT,
                        PropertyIdentifier.ACTIVE_TEXT,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.BINARY_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.INACTIVE_TEXT,
                        PropertyIdentifier.ACTIVE_TEXT,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                        PropertyIdentifier.FEEDBACK_VALUE,
                    }  
                },
                { ObjectTypes.BINARY_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.INACTIVE_TEXT,
                        PropertyIdentifier.ACTIVE_TEXT,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.STATE_TEXT ,
                        PropertyIdentifier.ALARM_VALUES,
                        PropertyIdentifier.FAULT_VALUES,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.STATE_TEXT ,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.FEEDBACK_VALUE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.STATE_TEXT ,
                        PropertyIdentifier.ALARM_VALUES,
                        PropertyIdentifier.FAULT_VALUES,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.ACCUMULATOR.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.SCALE ,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.HIGH_LIMIT,
                        PropertyIdentifier.LOW_LIMIT,
                        PropertyIdentifier.LIMIT_ENABLE,
                        PropertyIdentifier.NOTIFY_TYPE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.EVENT_ENABLE,
                    }  
                },
                { ObjectTypes.DEVICE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.SYSTEM_STATUS,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.FIRMWARE_REVISION,
                        PropertyIdentifier.PROTOCOL_VERSION,
                        PropertyIdentifier.PROTOCOL_REVISION,
                    }
                },
                { ObjectTypes.CALENDAR.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.DATE_LIST,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                    }  
                },
                { ObjectTypes.SCHEDULE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.EFFECTIVE_PERIOD,
                        PropertyIdentifier.EXCEPTION_SCHEDULE,
                        PropertyIdentifier.WEEKLY_SCHEDULE,
                        PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                    }  
                },
                { ObjectTypes.LIFE_SAFETY_POINT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.OBJECT_IDENTIFIER,
                        PropertyIdentifier.OBJECT_NAME,
                        PropertyIdentifier.OBJECT_TYPE,
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.TRACKING_VALUE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.MODE,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.RELIABILITY,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.SILENCED,
                    }
                },
                { ObjectTypes.LIFE_SAFETY_ZONE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.OBJECT_IDENTIFIER,
                        PropertyIdentifier.OBJECT_NAME,
                        PropertyIdentifier.OBJECT_TYPE,
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.TRACKING_VALUE,
                        PropertyIdentifier.DESCRIPTION,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.MODE,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.RELIABILITY,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.SILENCED,
                    }
                }
            };

        //import from file
    public static readonly Dictionary<string, List<PropertyIdentifier>> ObjectPropertyDictionaryRequired
        = new Dictionary<string, List<PropertyIdentifier>>
            {
                { ObjectTypes.ANALOG_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.ANALOG_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.ANALOG_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.BINARY_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.BINARY_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.BINARY_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_INPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_OUTPUT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.PRIORITY_ARRAY ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.MULTI_STATE_VALUE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.NUMBER_OF_STATES,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.ACCUMULATOR.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.SCALE ,
                        PropertyIdentifier.UNITS ,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.OUT_OF_SERVICE,
                    }  
                },
                { ObjectTypes.DEVICE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.SYSTEM_STATUS,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                        PropertyIdentifier.FIRMWARE_REVISION,
                        PropertyIdentifier.PROTOCOL_VERSION,
                        PropertyIdentifier.PROTOCOL_REVISION,
                    }
                },
                { ObjectTypes.CALENDAR.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.DATE_LIST,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                    }  
                },

                { ObjectTypes.SCHEDULE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.EFFECTIVE_PERIOD,
                        PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES,
                        PropertyIdentifier.OBJECT_IDENTIFIER ,
                        PropertyIdentifier.OBJECT_NAME ,
                        PropertyIdentifier.OBJECT_TYPE ,
                    }  
                },
                { ObjectTypes.LIFE_SAFETY_POINT.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.OBJECT_IDENTIFIER,
                        PropertyIdentifier.OBJECT_NAME,
                        PropertyIdentifier.OBJECT_TYPE,
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.TRACKING_VALUE,
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.MODE,                        
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.RELIABILITY,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.SILENCED,
                    }
                },
                { ObjectTypes.LIFE_SAFETY_ZONE.ToString(),
                    new List<PropertyIdentifier>( )
                    {
                        PropertyIdentifier.OBJECT_IDENTIFIER,
                        PropertyIdentifier.OBJECT_NAME,
                        PropertyIdentifier.OBJECT_TYPE,
                        PropertyIdentifier.PRESENT_VALUE,
                        PropertyIdentifier.TRACKING_VALUE,                        
                        PropertyIdentifier.STATUS_FLAGS,
                        PropertyIdentifier.MODE,                        
                        PropertyIdentifier.EVENT_STATE,
                        PropertyIdentifier.RELIABILITY,
                        PropertyIdentifier.OUT_OF_SERVICE,
                        PropertyIdentifier.SILENCED,
                    }
                }
            };

    public static readonly Dictionary<string, Dictionary<PropertyIdentifier, APPLICATION_TAG>> ObjectPropertyTypeDictionary
        = new Dictionary<string, Dictionary<PropertyIdentifier, APPLICATION_TAG>>
            {
                { ObjectTypes.ANALOG_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.UNITS,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.MIN_PRES_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.MAX_PRES_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.ANALOG_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.UNITS,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.MIN_PRES_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.MAX_PRES_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.ANALOG_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.UNITS,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.BINARY_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.BINARY_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.FEEDBACK_VALUE,
                            APPLICATION_TAG.BOOLEAN
                        },
                    }  
                },
                { ObjectTypes.BINARY_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.MULTI_STATE_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.ALARM_VALUES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.FAULT_VALUES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.MULTI_STATE_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.FEEDBACK_VALUE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.MULTI_STATE_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.ALARM_VALUES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.FAULT_VALUES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }  
                },
                { ObjectTypes.ACCUMULATOR.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.SCALE,
                            APPLICATION_TAG.REAL
                        },
                        {
                            PropertyIdentifier.UNITS,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.NOTIFY_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            APPLICATION_TAG.BIT_STRING
                        },
                    }
                },
                { ObjectTypes.DEVICE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.SYSTEM_STATUS,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.FIRMWARE_REVISION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.PROTOCOL_VERSION,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.PROTOCOL_REVISION,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.MAX_APDU_LENGTH_ACCEPTED,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.SEGMENTATION_SUPPORTED,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.VENDOR_IDENTIFIER,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.VENDOR_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        }
                    }
                },
                { ObjectTypes.CALENDAR.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.DATE_LIST,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                    }
                },

                { ObjectTypes.SCHEDULE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.DOUBLE
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.EFFECTIVE_PERIOD,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.EXCEPTION_SCHEDULE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.WEEKLY_SCHEDULE,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES,
                            APPLICATION_TAG.UNSIGNED_INT
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                    } 
                },

                { ObjectTypes.LIFE_SAFETY_POINT.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.TRACKING_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },                        
                        {
                            PropertyIdentifier.MODE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.RELIABILITY,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.SILENCED,
                            APPLICATION_TAG.ENUMERATED
                        },                        
                    }
                },

                { ObjectTypes.LIFE_SAFETY_ZONE.ToString(),
                    new Dictionary<PropertyIdentifier, APPLICATION_TAG>( )
                    {
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            APPLICATION_TAG.OBJECT_ID
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.TRACKING_VALUE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            APPLICATION_TAG.CHARACTER_STRING
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            APPLICATION_TAG.BIT_STRING
                        },
                        {
                            PropertyIdentifier.MODE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.RELIABILITY,
                            APPLICATION_TAG.ENUMERATED
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            APPLICATION_TAG.BOOLEAN
                        },
                        {
                            PropertyIdentifier.SILENCED,
                            APPLICATION_TAG.ENUMERATED
                        },
                    }
                },
            };

        public static readonly Dictionary<string, Dictionary<PropertyIdentifier, bool>> ObjectPropertyWritableDictionary
            = new Dictionary<string, Dictionary<PropertyIdentifier, bool>>
                {
                { ObjectTypes.ANALOG_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.UNITS,
                            false
                        },
                        {
                            PropertyIdentifier.MIN_PRES_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.MAX_PRES_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            true
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.ANALOG_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.UNITS,
                            false
                        },
                        {
                            PropertyIdentifier.MIN_PRES_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.MAX_PRES_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            false
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            true
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.ANALOG_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.UNITS,
                            false
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            false
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.DEADBAND,
                            true
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.BINARY_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.BINARY_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                        {
                            PropertyIdentifier.FEEDBACK_VALUE,
                            true
                        },
                    }
                },
                { ObjectTypes.BINARY_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.INACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.ACTIVE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.MULTI_STATE_INPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            false
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.ALARM_VALUES,
                            false
                        },
                        {
                            PropertyIdentifier.FAULT_VALUES,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.MULTI_STATE_OUTPUT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            false
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.PRIORITY_ARRAY,
                            false
                        },
                        {
                            PropertyIdentifier.FEEDBACK_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.MULTI_STATE_VALUE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.NUMBER_OF_STATES,
                            false
                        },
                        {
                            PropertyIdentifier.STATE_TEXT,
                            true
                        },
                        {
                            PropertyIdentifier.ALARM_VALUES,
                            false
                        },
                        {
                            PropertyIdentifier.FAULT_VALUES,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        },
                    }
                },
                { ObjectTypes.ACCUMULATOR.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            true
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.SCALE,
                            false
                        },
                        {
                            PropertyIdentifier.UNITS,
                            false
                        },
                        {
                            PropertyIdentifier.HIGH_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.LOW_LIMIT,
                            true
                        },
                        {
                            PropertyIdentifier.LIMIT_ENABLE,
                            true
                        },
                        {
                            PropertyIdentifier.NOTIFY_TYPE,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.EVENT_ENABLE,
                            true
                        }
                    }
                },
                { ObjectTypes.DEVICE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.SYSTEM_STATUS,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.FIRMWARE_REVISION,
                            false
                        },
                        {
                            PropertyIdentifier.PROTOCOL_VERSION,
                            false
                        },
                        {
                            PropertyIdentifier.PROTOCOL_REVISION,
                            false
                        },
                    }
                },
                { ObjectTypes.CALENDAR.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.DATE_LIST,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                    }
                },

                { ObjectTypes.SCHEDULE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.EFFECTIVE_PERIOD,
                            true
                        },
                        {
                            PropertyIdentifier.EXCEPTION_SCHEDULE,
                            true
                        },
                        {
                            PropertyIdentifier.WEEKLY_SCHEDULE,
                            true
                        },
                        {
                            PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES,
                            true
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            true
                        },
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                    }
                },

                { ObjectTypes.LIFE_SAFETY_POINT.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.TRACKING_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            false
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.MODE,
                            true
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.RELIABILITY,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.SILENCED,
                            false
                        },
                    }
                },

                { ObjectTypes.LIFE_SAFETY_ZONE.ToString(),
                    new Dictionary<PropertyIdentifier, bool>( )
                    {
                        {
                            PropertyIdentifier.OBJECT_IDENTIFIER,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_NAME,
                            false
                        },
                        {
                            PropertyIdentifier.OBJECT_TYPE,
                            false
                        },
                        {
                            PropertyIdentifier.PRESENT_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.TRACKING_VALUE,
                            false
                        },
                        {
                            PropertyIdentifier.DESCRIPTION,
                            false
                        },
                        {
                            PropertyIdentifier.STATUS_FLAGS,
                            false
                        },
                        {
                            PropertyIdentifier.MODE,
                            true
                        },
                        {
                            PropertyIdentifier.EVENT_STATE,
                            false
                        },
                        {
                            PropertyIdentifier.RELIABILITY,
                            false
                        },
                        {
                            PropertyIdentifier.OUT_OF_SERVICE,
                            false
                        },
                        {
                            PropertyIdentifier.SILENCED,
                            false
                        },
                    }
                },
            };

    public static readonly Dictionary<BACnetEnums.APPLICATION_TAG, uint> ApplicationTagBuiltInType = new Dictionary<BACnetEnums.APPLICATION_TAG, uint>()
    {
         {
             APPLICATION_TAG.BOOLEAN,
             (uint)BuiltInType.Boolean
         },
         {
             APPLICATION_TAG.UNSIGNED_INT,
             (uint)BuiltInType.UInt32
         },
         {
             APPLICATION_TAG.SIGNED_INT,
             (uint)BuiltInType.Int32
         },
         {
             APPLICATION_TAG.REAL,
             (uint)BuiltInType.Float
         },
         {
             APPLICATION_TAG.DOUBLE,
             (uint)BuiltInType.Double
         },
         {
             APPLICATION_TAG.OCTET_STRING,
             (uint)BuiltInType.Byte
         },
         {
             APPLICATION_TAG.BIT_STRING,
             (uint)BuiltInType.Byte
         },
         {
             APPLICATION_TAG.CHARACTER_STRING,
             (uint)BuiltInType.String
         },
         {
             APPLICATION_TAG.OBJECT_ID,
             (uint)BuiltInType.UInt32
         },
         {
             APPLICATION_TAG.ENUMERATED,
             (uint)BuiltInType.UInt16
         },
         {
             APPLICATION_TAG.DATE,
             (uint)BuiltInType.UInt32
         },
         {
             APPLICATION_TAG.TIME,
             (uint)BuiltInType.UInt32
         },
    };

    public static readonly Dictionary<BACnetEnums.APPLICATION_TAG, UFUAModel.DataType> ApplicationTagDataType = new Dictionary<BACnetEnums.APPLICATION_TAG, UFUAModel.DataType>()
    {
         {
             APPLICATION_TAG.BOOLEAN,
             UFUAModel.DataType.Boolean
         },
         {
             APPLICATION_TAG.UNSIGNED_INT,
             UFUAModel.DataType.UInt32
         },
         {
             APPLICATION_TAG.SIGNED_INT,
             UFUAModel.DataType.Int32
         },
         {
             APPLICATION_TAG.REAL,
             UFUAModel.DataType.Float
         },
         {
             APPLICATION_TAG.DOUBLE,
             UFUAModel.DataType.Double
         },
         {
             APPLICATION_TAG.OCTET_STRING,
             UFUAModel.DataType.Byte
         },
         {
             APPLICATION_TAG.BIT_STRING,
             UFUAModel.DataType.Byte
         },
         {
             APPLICATION_TAG.CHARACTER_STRING,
             UFUAModel.DataType.String
         },
         {
             APPLICATION_TAG.OBJECT_ID,
             UFUAModel.DataType.UInt32
         },
         {
             APPLICATION_TAG.ENUMERATED,
             UFUAModel.DataType.UInt16
         },
         {
             APPLICATION_TAG.DATE,
             UFUAModel.DataType.UInt32
         },
         {
             APPLICATION_TAG.TIME,
             UFUAModel.DataType.UInt32
         },
    };
    public static int repeatDelay = 1000;
    public static int WaitWhoIs = 100;

    public enum BACnet_SEGMENTATION
    {
      SEGMENTATION_BOTH = 0,
      SEGMENTATION_TRANSMIT = 1,
      SEGMENTATION_RECEIVE = 2,
      SEGMENTATION_NONE = 3,
      MAX_BACnet_SEGMENTATION = 4
    }

    public enum BACnet_VT_CLASS
    {
      VT_CLASS_DEFAULT = 0,
      VT_CLASS_ANSI_X34 = 1,      /* real name is ANSI X3.64 */
      VT_CLASS_DEC_VT52 = 2,
      VT_CLASS_DEC_VT100 = 3,
      VT_CLASS_DEC_VT220 = 4,
      VT_CLASS_HP_700_94 = 5,     /* real name is HP 700/94 */
      VT_CLASS_IBM_3130 = 6
      /* Enumerated values 0-63 are reserved for definition by ASHRAE.  */
      /* Enumerated values 64-65535 may be used by others subject to  */
      /* the procedures and constraints described in Clause 23. */
    }

    public enum BACnet_CHARACTER_STRING_ENCODING
    {
      CHARACTER_ANSI_X34 = 0,
      CHARACTER_MS_DBCS = 1,
      CHARACTER_JISC_6226 = 2,
      CHARACTER_UCS4 = 3,
      CHARACTER_UCS2 = 4,
      CHARACTER_ISO8859 = 5
    }

    public enum TYPE_TAG
    {
        noType = 0,
        opening = 6,
        closing = 7,
    }

    public enum APPLICATION_TAG
    {
      NULL = 0,
      BOOLEAN = 1,
      UNSIGNED_INT = 2,
      SIGNED_INT = 3,
      REAL = 4,
      DOUBLE = 5,
      OCTET_STRING = 6,
      CHARACTER_STRING = 7,
      BIT_STRING = 8,
      ENUMERATED = 9,
      DATE = 10,
      TIME = 11,
      OBJECT_ID = 12,
      RESERVE1 = 13,
      RESERVE2 = 14,
      RESERVE3 = 15,
      MAX_TAG = 16
    }

    /* note: these are not the real values, */
    /* but are shifted left for easy encoding */
    public enum BACnetPDU
    {
      CONFIRMED_REQUEST = 0,
      UNCONFIRMED_REQUEST = 0x10,
      SIMPLE_ACK = 0x20,
      COMPLEX_ACK = 0x30,
      SEGMENT_ACK = 0x40,
      ERROR = 0x50,
      REJECT = 0x60,
      ABORT = 0x70
    }

    public enum ConfirmedService
    {
      /* Alarm and Event Services */
      ACKNOWLEDGE_ALARM = 0,
      COV_NOTIFICATION = 1,
      EVENT_NOTIFICATION = 2,
      GET_ALARM_SUMMARY = 3,
      GET_ENROLLMENT_SUMMARY = 4,
      GET_EVENT_INFORMATION = 29,
      SUBSCRIBE_COV = 5,
      SUBSCRIBE_COV_PROPERTY = 28,
      LIFE_SAFETY_OPERATION = 27,
      /* File Access Services */
      ATOMIC_READ_FILE = 6,
      ATOMIC_WRITE_FILE = 7,
      /* Object Access Services */
      ADD_LIST_ELEMENT = 8,
      REMOVE_LIST_ELEMENT = 9,
      CREATE_OBJECT = 10,
      DELETE_OBJECT = 11,
      READ_PROPERTY = 12,
      READ_PROP_CONDITIONAL = 13,
      READ_PROP_MULTIPLE = 14,
      READ_RANGE = 26,
      WRITE_PROPERTY = 15,
      WRITE_PROP_MULTIPLE = 16,
      /* Remote Device Management Services */
      DEVICE_COMMUNICATION_CONTROL = 17,
      PRIVATE_TRANSFER = 18,
      TEXT_MESSAGE = 19,
      REINITIALIZE_DEVICE = 20,
      /* Virtual Terminal Services */
      VT_OPEN = 21,
      VT_CLOSE = 22,
      VT_DATA = 23,
      /* Security Services */
      AUTHENTICATE = 24,
      REQUEST_KEY = 25,
      /* Services added after 1995 */
      /* readRange (26) see Object Access Services */
      /* lifeSafetyOperation (27) see Alarm and Event Services */
      /* subscribeCOVProperty (28) see Alarm and Event Services */
      /* getEventInformation (29) see Alarm and Event Services */
      MAX_BACnet_CONFIRMED_SERVICE = 30
    }

    public enum UnconfirmedService
    {
      I_AM = 0,
      I_HAVE = 1,
      COV_NOTIFICATION = 2,
      EVENT_NOTIFICATION = 3,
      PRIVATE_TRANSFER = 4,
      TEXT_MESSAGE = 5,
      TIME_SYNCHRONIZATION = 6,
      WHO_HAS = 7,
      WHO_IS = 8,
      UTC_TIME_SYNCHRONIZATION = 9,
      /* Other services to be added as they are defined. */
      /* All choice values in this production are reserved */
      /* for definition by ASHRAE. */
      /* Proprietary extensions are made by using the */
      /* UnconfirmedPrivateTransfer service. See Clause 23. */
      MAX_BACnet_UNCONFIRMED_SERVICE = 10
    }

    /* Bit String Enumerations */
    public enum ServiceSupported
    {
      /* Alarm and Event Services */
      ACKNOWLEDGE_ALARM = 0,
      CONFIRMED_COV_NOTIFICATION = 1,
      CONFIRMED_EVENT_NOTIFICATION = 2,
      GET_ALARM_SUMMARY = 3,
      GET_ENROLLMENT_SUMMARY = 4,
      GET_EVENT_INFORMATION = 39,
      SUBSCRIBE_COV = 5,
      SUBSCRIBE_COV_PROPERTY = 38,
      LIFE_SAFETY_OPERATION = 37,
      /* File Access Services */
      ATOMIC_READ_FILE = 6,
      ATOMIC_WRITE_FILE = 7,
      /* Object Access Services */
      ADD_LIST_ELEMENT = 8,
      REMOVE_LIST_ELEMENT = 9,
      CREATE_OBJECT = 10,
      DELETE_OBJECT = 11,
      READ_PROPERTY = 12,
      READ_PROP_CONDITIONAL = 13,
      READ_PROP_MULTIPLE = 14,
      READ_RANGE = 35,
      WRITE_PROPERTY = 15,
      WRITE_PROP_MULTIPLE = 16,
      /* Remote Device Management Services */
      DEVICE_COMMUNICATION_CONTROL = 17,
      PRIVATE_TRANSFER = 18,
      TEXT_MESSAGE = 19,
      REINITIALIZE_DEVICE = 20,
      /* Virtual Terminal Services */
      SUPPORTED_VT_OPEN = 21,
      SUPPORTED_VT_CLOSE = 22,
      SUPPORTED_VT_DATA = 23,
      /* Security Services */
      AUTHENTICATE = 24,
      REQUEST_KEY = 25,
      I_AM = 26,
      I_HAVE = 27,
      UNCONFIRMED_COV_NOTIFICATION = 28,
      UNCONFIRMED_EVENT_NOTIFICATION = 29,
      UNCONFIRMED_PRIVATE_TRANSFER = 30,
      UNCONFIRMED_TEXT_MESSAGE = 31,
      TIME_SYNCHRONIZATION = 32,
      UTC_TIME_SYNCHRONIZATION = 36,
      WHO_HAS = 33,
      WHO_IS = 34,
      /* Other services to be added as they are defined. */
      /* All values in this production are reserved */
      /* for definition by ASHRAE. */
      MAX_BACnet_SERVICES_SUPPORTED = 40
    }

    public enum BACnet_BVLC_FUNCTION
    {
      BVLC_RESULT = 0,
      BVLC_WRITE_BROADCAST_DISTRIBUTION_TABLE = 1,
      BVLC_READ_BROADCAST_DIST_TABLE = 2,
      BVLC_READ_BROADCAST_DIST_TABLE_ACK = 3,
      BVLC_FORWARDED_NPDU = 4,
      BVLC_REGISTER_FOREIGN_DEVICE = 5,
      BVLC_READ_FOREIGN_DEVICE_TABLE = 6,
      BVLC_READ_FOREIGN_DEVICE_TABLE_ACK = 7,
      BVLC_DELETE_FOREIGN_DEVICE_TABLE_ENTRY = 8,
      BVLC_DISTRIBUTE_BROADCAST_TO_NETWORK = 9,
      BVLC_ORIGINAL_UNICAST_NPDU = 10,
      BVLC_ORIGINAL_BROADCAST_NPDU = 11,
      MAX_BVLC_FUNCTION = 12
    }

    public enum BACnet_BVLC_RESULT
    {
      BVLC_RESULT_SUCCESSFUL_COMPLETION = 0x0000,
      BVLC_RESULT_WRITE_BROADCAST_DISTRIBUTION_TABLE_NAK = 0x0010,
      BVLC_RESULT_READ_BROADCAST_DISTRIBUTION_TABLE_NAK = 0x0020,
      BVLC_RESULT_REGISTER_FOREIGN_DEVICE_NAK = 0X0030,
      BVLC_RESULT_READ_FOREIGN_DEVICE_TABLE_NAK = 0x0040,
      BVLC_RESULT_DELETE_FOREIGN_DEVICE_TABLE_ENTRY_NAK = 0x0050,
      BVLC_RESULT_DISTRIBUTE_BROADCAST_TO_NETWORK_NAK = 0x0060
    }

    /* Bit String Enumerations */
    public enum BACnetStatusFlags
    {
      IN_ALARM = 0,
      FAULT = 1,
      OVERRIDDEN = 2,
      OUT_OF_SERVICE = 3
    }

    public enum BACnet_ACKNOWLEDGMENT_FILTER
    {
      ACKNOWLEDGMENT_FILTER_ALL = 0,
      ACKNOWLEDGMENT_FILTER_ACKED = 1,
      ACKNOWLEDGMENT_FILTER_NOT_ACKED = 2
    }

    public enum BACnet_EVENT_STATE_FILTER
    {
      EVENT_STATE_FILTER_OFFNORMAL = 0,
      EVENT_STATE_FILTER_FAULT = 1,
      EVENT_STATE_FILTER_NORMAL = 2,
      EVENT_STATE_FILTER_ALL = 3,
      EVENT_STATE_FILTER_ACTIVE = 4
    }

    public enum BACnet_SELECTION_LOGIC
    {
      SELECTION_LOGIC_AND = 0,
      SELECTION_LOGIC_OR = 1,
      SELECTION_LOGIC_ALL = 2
    }

    public enum BACnet_RELATION_SPECIFIER
    {
      RELATION_SPECIFIER_EQUAL = 0,
      RELATION_SPECIFIER_NOT_EQUAL = 1,
      RELATION_SPECIFIER_LESS_THAN = 2,
      RELATION_SPECIFIER_GREATER_THAN = 3,
      RELATION_SPECIFIER_LESS_THAN_OR_EQUAL = 4,
      RELATION_SPECIFIER_GREATER_THAN_OR_EQUAL = 5
    }

    public enum BACnet_COMMUNICATION_ENABLE_DISABLE
    {
      COMMUNICATION_ENABLE = 0,
      COMMUNICATION_DISABLE = 1,
      COMMUNICATION_DISABLE_INITIATION = 2,
      MAX_BACnet_COMMUNICATION_ENABLE_DISABLE = 3
    }

    public enum NetworkPriority
    {
      NORMAL = 0,
      URGENT = 1,
      CRITICAL_EQUIPMENT = 2,
      LIFE_SAFETY = 3
    }

    /*Network Layer Message Type */
    /*If Bit 7 of the control octet described in 6.2.2 is 1, */
    /* a message type octet shall be present as shown in Figure 6-1. */
    /* The following message types are indicated: */
    public enum BACnet_NETWORK_MESSAGE_TYPE
    {
      NETWORK_MESSAGE_WHO_IS_ROUTER_TO_NETWORK = 0,
      NETWORK_MESSAGE_I_AM_ROUTER_TO_NETWORK = 1,
      NETWORK_MESSAGE_I_COULD_BE_ROUTER_TO_NETWORK = 2,
      NETWORK_MESSAGE_REJECT_MESSAGE_TO_NETWORK = 3,
      NETWORK_MESSAGE_ROUTER_BUSY_TO_NETWORK = 4,
      NETWORK_MESSAGE_ROUTER_AVAILABLE_TO_NETWORK = 5,
      NETWORK_MESSAGE_INIT_RT_TABLE = 6,
      NETWORK_MESSAGE_INIT_RT_TABLE_ACK = 7,
      NETWORK_MESSAGE_ESTABLISH_CONNECTION_TO_NETWORK = 8,
      NETWORK_MESSAGE_DISCONNECT_CONNECTION_TO_NETWORK = 9,
      /* X'0A' to X'7F': Reserved for use by ASHRAE, */
      /* X'80' to X'FF': Available for vendor proprietary messages */
      NETWORK_MESSAGE_INVALID = 0x100
    }


    public enum BACnet_REINITIALIZED_STATE_OF_DEVICE
    {
      REINITIALIZED_STATE_COLD_START = 0,
      REINITIALIZED_STATE_WARM_START = 1,
      REINITIALIZED_STATE_START_BACKUP = 2,
      REINITIALIZED_STATE_END_BACKUP = 3,
      REINITIALIZED_STATE_START_RESTORE = 4,
      REINITIALIZED_STATE_END_RESTORE = 5,
      REINITIALIZED_STATE_ABORT_RESTORE = 6,
      REINITIALIZED_STATE_IDLE = 255
    }

    public enum BACnet_ABORT_REASON
    {
      ABORT_REASON_OTHER = 0,
      ABORT_REASON_BUFFER_OVERFLOW = 1,
      ABORT_REASON_INVALID_APDU_IN_THIS_STATE = 2,
      ABORT_REASON_PREEMPTED_BY_HIGHER_PRIORITY_TASK = 3,
      ABORT_REASON_SEGMENTATION_NOT_SUPPORTED = 4,
      /* Enumerated values 0-63 are reserved for definition by ASHRAE. */
      /* Enumerated values 64-65535 may be used by others subject to */
      /* the procedures and constraints described in Clause 23. */
      MAX_BACnet_ABORT_REASON = 5,
      FIRST_PROPRIETARY_ABORT_REASON = 64,
      LAST_PROPRIETARY_ABORT_REASON = 65535
    }

    public enum BACnet_BACnet_REJECT_REASON
    {
      REJECT_REASON_OTHER = 0,
      REJECT_REASON_BUFFER_OVERFLOW = 1,
      REJECT_REASON_INCONSISTENT_PARAMETERS = 2,
      REJECT_REASON_INVALID_PARAMETER_DATA_TYPE = 3,
      REJECT_REASON_INVALID_TAG = 4,
      REJECT_REASON_MISSING_REQUIRED_PARAMETER = 5,
      REJECT_REASON_PARAMETER_OUT_OF_RANGE = 6,
      REJECT_REASON_TOO_MANY_ARGUMENTS = 7,
      REJECT_REASON_UNDEFINED_ENUMERATION = 8,
      REJECT_REASON_UNRECOGNIZED_SERVICE = 9,
      /* Enumerated values 0-63 are reserved for definition by ASHRAE. */
      /* Enumerated values 64-65535 may be used by others subject to */
      /* the procedures and constraints described in Clause 23. */
      MAX_BACnet_REJECT_REASON = 10,
      FIRST_PROPRIETARY_REJECT_REASON = 64,
      LAST_PROPRIETARY_REJECT_REASON = 65535
    }

    public enum ERROR_CLASS
    {
      DEVICE = 0,
      OBJECT = 1,
      PROPERTY = 2,
      RESOURCES = 3,
      SECURITY = 4,
      SERVICES = 5,
      VT = 6,
      /* Enumerated values 0-63 are reserved for definition by ASHRAE. */
      /* Enumerated values 64-65535 may be used by others subject to */
      /* the procedures and constraints described in Clause 23. */
      MAX = 7,
      FIRST_PROPRIETARY_ERROR_CLASS = 64,
      LAST_PROPRIETARY_ERROR_CLASS = 65535
    }

    /* These are sorted in the order given in
       Clause 18. ERROR, REJECT AND ABORT CODES
       The Class and Code pairings are required
       to be used in accordance with Clause 18. */
    public enum ERROR_CODE
    {
      /* valid for all classes */
      OTHER = 0,

      /* Error Class - Device */
      DEVICE_BUSY = 3,
      CONFIGURATION_IN_PROGRESS = 2,
      OPERATIONAL_PROBLEM = 25,

      /* Error Class - Object */
      DYNAMIC_CREATION_NOT_SUPPORTED = 4,
      NO_OBJECTS_OF_SPECIFIED_TYPE = 17,
      OBJECT_DELETION_NOT_PERMITTED = 23,
      OBJECT_IDENTIFIER_ALREADY_EXISTS = 24,
      READ_ACCESS_DENIED = 27,
      UNKNOWN_OBJECT = 31,
      UNSUPPORTED_OBJECT_TYPE = 36,

      /* Error Class - Property */
      CHARACTER_SET_NOT_SUPPORTED = 41,
      DATATYPE_NOT_SUPPORTED = 47,
      INCONSISTENT_SELECTION_CRITERION = 8,
      INVALID_ARRAY_INDEX = 42,
      INVALID_DATA_TYPE = 9,
      NOT_COV_PROPERTY = 44,
      OPTIONAL_FUNCTIONALITY_NOT_SUPPORTED = 45,
      PROPERTY_IS_NOT_AN_ARRAY = 50,
      /* ERROR_CODE_READ_ACCESS_DENIED = 27, */
      UNKNOWN_PROPERTY = 32,
      VALUE_OUT_OF_RANGE = 37,
      WRITE_ACCESS_DENIED = 40,

      /* Error Class - Resources */
      NO_SPACE_FOR_OBJECT = 18,
      NO_SPACE_TO_ADD_LIST_ELEMENT = 19,
      NO_SPACE_TO_WRITE_PROPERTY = 20,

      /* Error Class - Security */
      AUTHENTICATION_FAILED = 1,
      /* ERROR_CODE_CHARACTER_SET_NOT_SUPPORTED = 41, */
      INCOMPATIBLE_SECURITY_LEVELS = 6,
      INVALID_OPERATOR_NAME = 12,
      KEY_GENERATION_ERROR = 15,
      PASSWORD_FAILURE = 26,
      SECURITY_NOT_SUPPORTED = 28,
      TIMEOUT = 30,

      /* Error Class - Services */
      /* ERROR_CODE_CHARACTER_SET_NOT_SUPPORTED = 41, */
      COV_SUBSCRIPTION_FAILED = 43,
      DUPLICATE_NAME = 48,
      DUPLICATE_OBJECT_ID = 49,
      FILE_ACCESS_DENIED = 5,
      INCONSISTENT_PARAMETERS = 7,
      INVALID_CONFIGURATION_DATA = 46,
      INVALID_FILE_ACCESS_METHOD = 10,
      ERROR_CODE_INVALID_FILE_START_POSITION = 11,
      INVALID_PARAMETER_DATA_TYPE = 13,
      INVALID_TIME_STAMP = 14,
      MISSING_REQUIRED_PARAMETER = 16,
      /* ERROR_CODE_OPTIONAL_FUNCTIONALITY_NOT_SUPPORTED = 45, */
      CODE_PROPERTY_IS_NOT_A_LIST = 22,
      CODE_SERVICE_REQUEST_DENIED = 29,

      /* Error Class - VT */
      CODE_UNKNOWN_VT_CLASS = 34,
      CODE_UNKNOWN_VT_SESSION = 35,
      CODE_NO_VT_SESSIONS_AVAILABLE = 21,
      CODE_VT_SESSION_ALREADY_CLOSED = 38,
      CODE_VT_SESSION_TERMINATION_FAILURE = 39,

      /* unused */
      CODE_RESERVED1 = 33,
      /* Enumerated values 0-255 are reserved for definition by ASHRAE. */
      /* Enumerated values 256-65535 may be used by others subject to */
      /* the procedures and constraints described in Clause 23. */
      /* The last enumeration used in this version is 50. */
      MAX = 51,
      FIRST_PROPRIETARY_ERROR_CODE = 256,
      LAST_PROPRIETARY_ERROR_CODE = 65535
    }

    public enum BACnet_REINITIALIZED_STATE
    {
      BACnet_REINIT_COLDSTART = 0,
      BACnet_REINIT_WARMSTART = 1,
      BACnet_REINIT_STARTBACKUP = 2,
      BACnet_REINIT_ENDBACKUP = 3,
      BACnet_REINIT_STARTRESTORE = 4,
      BACnet_REINIT_ENDRESTORE = 5,
      BACnet_REINIT_ABORTRESTORE = 6,
      MAX_BACnet_REINITIALIZED_STATE = 7
    }

    public enum BACnet_NODE_TYPE
    {
      BACnet_NODE_UNKNOWN = 0,
      BACnet_NODE_SYSTEM = 1,
      BACnet_NODE_NETWORK = 2,
      BACnet_NODE_DEVICE = 3,
      BACnet_NODE_ORGANIZATIONAL = 4,
      BACnet_NODE_AREA = 5,
      BACnet_NODE_EQUIPMENT = 6,
      BACnet_NODE_POINT = 7,
      BACnet_NODE_COLLECTION = 8,
      BACnet_NODE_PROPERTY = 9,
      BACnet_NODE_FUNCTIONAL = 10,
      BACnet_NODE_OTHER = 11
    }

    public enum BACnet_SHED_STATE
    {
      BACnet_SHED_INACTIVE = 0,
      BACnet_SHED_REQUEST_PENDING = 1,
      BACnet_SHED_COMPLIANT = 2,
      BACnet_SHED_NON_COMPLIANT = 3
    }

    public enum BACnet_LIGHTING_OPERATION
    {
      BACnet_LIGHTS_STOP = 0,
      BACnet_LIGHTS_FADE_TO = 1,
      BACnet_LIGHTS_FADE_TO_OVER = 2,
      BACnet_LIGHTS_RAMP_TO = 3,
      BACnet_LIGHTS_RAMP_TO_AT_RATE = 4,
      BACnet_LIGHTS_RAMP_UP = 5,
      BACnet_LIGHTS_RAMP_UP_AT_RATE = 6,
      BACnet_LIGHTS_RAMP_DOWN = 7,
      BACnet_LIGHTS_RAMP_DOWN_AT_RATE = 8,
      BACnet_LIGHTS_STEP_UP = 9,
      BACnet_LIGHTS_STEP_DOWN = 10,
      BACnet_LIGHTS_STEP_UP_BY = 11,
      BACnet_LIGHTS_STEP_DOWN_BY = 12,
      BACnet_LIGHTS_GOTO_LEVEL = 13,
      BACnet_LIGHTS_RELINQUISH = 14
    }

    /* NOTE: BACnet_DAYS_OF_WEEK is different than BACnet_WEEKDAY */
    /* 0=Monday-6=Sunday */
    public enum BACnet_DAYS_OF_WEEK
    {
      BACnet_DAYS_OF_WEEK_MONDAY = 0,
      BACnet_DAYS_OF_WEEK_TUESDAY = 1,
      BACnet_DAYS_OF_WEEK_WEDNESDAY = 2,
      BACnet_DAYS_OF_WEEK_THURSDAY = 3,
      BACnet_DAYS_OF_WEEK_FRIDAY = 4,
      BACnet_DAYS_OF_WEEK_SATURDAY = 5,
      BACnet_DAYS_OF_WEEK_SUNDAY = 6
    }

    public enum CalendarEntryTags
    {
        Date = 0,
        DateRange = 1,
        WeekNDay = 2,
    }
    public enum ObjectPropertyReferencesTags
    {
        objectIdentifier = 0,
        propertyIdentifier = 1,
        propertyArrayIndex = 2,
        deviceIdentifier = 3,
    }
    public enum ExceptionScheduleTags
    {
        periodCalendarEntry = 0,
        periodCalendarReference = 1,
        listOfTimeValues = 2,
        eventPriority = 3,
    }

    public enum ImportVarNameFormat : int
    {
        Keyname,
        ObjectName,
        ObjectName_PropertyName_KeyName
    }
  }
}
